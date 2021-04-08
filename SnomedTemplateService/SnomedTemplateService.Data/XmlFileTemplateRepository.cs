using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnomedTemplateService.Core.Domain;
using SnomedTemplateService.Core.Interfaces;

namespace SnomedTemplateService.Data
{
    public class XmlFileTemplateRepository : ITemplateRepository
    {
        private readonly TemplateCollection templateCollection;
        private readonly ILogger<XmlFileTemplateRepository> logger;
        private const string templatesCacheKey = "SNOMEDTEMPLATES";

        public XmlFileTemplateRepository(
            IMemoryCache memoryCache,
            IHostEnvironment hostEnvironment,
            IConfiguration configuration,
            ILogger<XmlFileTemplateRepository> logger,
            IEtlParseService etlParser
            )
        {
            this.logger = logger;
            if (!memoryCache.TryGetValue(templatesCacheKey, out templateCollection))
            {
                logger.LogInformation("Refreshing templates-dictionary.");
                var templateDirectoryPath = GetTemplateDirectoryPath(configuration);
                var templateDirectoryContents = GetTemplateDirectoryContents(templateDirectoryPath, hostEnvironment);
                var foundErrorsInXml = !GetTemplateDictionary(templateDirectoryContents, out var templatesMutableDictionary);
                var nrOfItemsBeforeCheckingEtl = templatesMutableDictionary.Count;
                templatesMutableDictionary = templatesMutableDictionary.Where(
                    kv =>
                    {
                        var foundErrorsInEtl = false;
                        try
                        {
                            etlParser.ParseExpressionTemplate(kv.Value.Etl);
                        }
                        catch (Exception e)
                        {
                            foundErrorsInEtl = true;
                            logger.LogError(e, "The template with key={key} contains an ETL syntax error", kv.Key);
                            templatesMutableDictionary.Remove(kv.Key);
                        }
                        return !foundErrorsInEtl;
                    }
                ).ToDictionary(kv => kv.Key, kv => kv.Value);
                var foundErrorsInTemplates = foundErrorsInXml || (nrOfItemsBeforeCheckingEtl != templatesMutableDictionary.Count);
                templateCollection = new TemplateCollection(templatesMutableDictionary, foundErrorsInTemplates);
                memoryCache.Set(templatesCacheKey, templateCollection, hostEnvironment.ContentRootFileProvider.Watch($"{templateDirectoryPath}\\*\\*.xml"));
            }
        }

        public bool FoundErrorsInTemplates
        {
            get
            {
                return templateCollection.FoundErrorsInTemplates;
            }
        }

        private string GetTemplateDirectoryPath(IConfiguration configuration)
        {
            return configuration["SnomedTemplatesDirectory"]?.TrimEnd('\\') ?? "SnomedTemplates";
        }

        private IDirectoryContents GetTemplateDirectoryContents(string path, IHostEnvironment hostEnvironment)
        {
            var result = hostEnvironment.ContentRootFileProvider.GetDirectoryContents(path);
            if (!result.Exists)
            {
                logger.LogError("The Templates Directory ({templateDirName}) doesn't exist.", path);
                throw new Exception($"The Templates Directory ({path}) doesn't exist.");
            }
            return result;
        }

        private bool GetTemplateDictionary(IDirectoryContents templateDirectory, out Dictionary<string, TemplateData> templates)
        {
            bool correct = true;
            templates = new Dictionary<string, TemplateData>();
            TagCollection tagCollection;
            try
            {
                var tagsFileInfo = templateDirectory.SingleOrDefault(f => string.Equals(f.Name, "tags.xml", StringComparison.OrdinalIgnoreCase));
                tagCollection = new TagCollection(tagsFileInfo, logger);
            }
            catch
            {
                correct = false;
                tagCollection = new TagCollection(logger);
                logger.LogError("Error in tags.xml");
            }
            foreach (var subdir in templateDirectory.Where(c => c.IsDirectory))
            {
                try
                {
                    if (subdir.Name.Contains("_"))
                    {
                        logger.LogError("The name of the template directory '{SubdirectoryName}' contains a underscore.", subdir.Name);
                        correct = false;
                    }
                    else
                    {
                        using var subDirFileProvider = new PhysicalFileProvider(subdir.PhysicalPath);
                        foreach (
                            var file in subDirFileProvider.GetDirectoryContents("")
                            .Where(f => !f.IsDirectory && f.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                            )
                        {
                            try
                            {
                                var match = Regex.Match(file.Name, @"^(.*?_((?:-|\+)?(?:0|[0-9]\d*)))\.xml$", RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    var key = $"{subdir.Name}_{match.Groups[1].Value}";
                                    using var fileStream = file.CreateReadStream();
                                    (var templateData, var tags) = GetTemplateData(key, match.Groups[2].Value, fileStream, tagCollection);
                                    templateData.Tags = tags;
                                    templates[key] = templateData;
                                }
                                else
                                {
                                    logger.LogError("The file name of {subDirName}\\{fileName} doesn't end with an unix timestamp", subdir.Name, file.Name);
                                    correct = false;
                                }
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, "Error in template file {subDirName}\\{fileName}", subdir.Name, file.Name);
                                correct = false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error in template directory {subDirName}\\.", subdir.Name);
                    correct = false;
                }
            }
            return correct;
        }

        public TemplateData GetById(string id)
        {
            if (templateCollection.Templates.TryGetValue(id, out var result))
            {
                return result;
            }
            return null;
        }

        public IList<TemplateData> GetTemplates()
        {
            return templateCollection.Templates.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
        }


        private (TemplateData templateData, IList<MultiLanguageString> tags) GetTemplateData(string key, string timestamp, Stream stream, TagCollection allTags)
        {
            var doc = new XmlDocument();
            doc.Load(stream);
            var templateNode = doc.SelectSingleNode("/template");

            var tags = new List<MultiLanguageString>();

            foreach (var element in templateNode.SelectNodes("tags/tag").Cast<XmlElement>())
            {
                var id = element.GetAttribute("id");
                if (string.IsNullOrEmpty(id))
                {
                    logger.LogError("No id specified for tag-element of template.");
                    throw new Exception("No id specified for tag-element of template.");
                }
                var tag = allTags.GetTagById(id);
                if (tag == null)
                {
                    logger.LogError("Tag with id='{id}' is not found.", id);
                    throw new Exception($"Tag with id='{id}' is not found.");
                }
                tags.Add(tag);
            }

            if (tags.Count == 0)
            {
                logger.LogError("A template should have at least one tag.");
                throw new Exception("A template should have at least one tag.");
            }

            var title = ConvertMultiLanguageElement(templateNode.SelectSingleNode("title"), "/template/title", logger);

            var result = new TemplateData(
                key,
                timestamp,
                templateNode.SelectSingleNode("snomedVersion")?.InnerText?.Trim(),
                templateNode.SelectSingleNode("snomedBranch")?.InnerText?.Trim(),
                title,
                templateNode.SelectSingleNode("etl")?.InnerText?.Trim()
            )
            {
                Description = ConvertMultiLanguageElement(templateNode.SelectSingleNode("description"), "/template/description", logger)
            };

            result.StringFormat = ConvertMultiLanguageElement(templateNode.SelectSingleNode("stringFormat"), "/template/stringFormat", logger);
            result.ItemData = templateNode.SelectNodes("items/item").Cast<XmlElement>()
                .ToDictionary(
                    e => e.GetAttribute("name"),
                    e => new TemplateData.Item(
                        ConvertMultiLanguageElement(e.SelectSingleNode("title"), "/template/items/item/title", logger),
                        ConvertMultiLanguageElement(e.SelectSingleNode("description"), "/template/item/description", logger)
                    )
                );

            result.Authors = templateNode.SelectNodes("authors/author")
                .Cast<XmlElement>()
                .Select(a => new TemplateData.Author(a.SelectSingleNode("name")?.InnerText?.Trim())
                {
                    Contact = a.SelectSingleNode("contact")?.InnerText?.Trim()
                }).ToList();

            return (result, tags);
        }
    
        private static MultiLanguageString ConvertMultiLanguageElement(XmlNode node, string path, ILogger<XmlFileTemplateRepository> logger)
        {
            if (node == null)
            {
                return null;
            }
            if (!node.ChildNodes.Cast<XmlNode>().All(n => n is XmlElement element && element.LocalName == "txt"))
            {
                logger.LogError("All children of '{elementPath}' must be txt-elements.", path);
                throw new Exception($"All children of '{path}' must be txt-elements.");
            }

            if (!node.ChildNodes.Cast<XmlElement>().All(e => e.HasAttribute("lang") && e.GetAttribute("lang").Length > 0))
            {
                logger.LogError("Each child of '{elementPath}' must have a 'lang'-attribute.", path);
                throw new Exception($"Each child of '{path}' must have a 'lang'-attribute.");
            }

            if (!node.ChildNodes.Cast<XmlElement>().All(e => !e.IsEmpty && (e.InnerText?.Trim().Length ?? 0) > 0 ))
            {
                logger.LogError("Each child of '{elementPath}' must be non-empty.", path);
                throw new Exception($"Each child of '{path}' must be non-empty.");
            }
            return new MultiLanguageString(
                node.ChildNodes.Cast<XmlElement>().ToDictionary(e => e.GetAttribute("lang"), 
                e => e.InnerText.Trim()
                )
            );
        }
        private class TemplateCollection
        {
            public TemplateCollection(IDictionary<string, TemplateData> templates, bool foundErrorsInTemplates)
            {
                Templates = templates.ToImmutableDictionary();
                FoundErrorsInTemplates = foundErrorsInTemplates;
            }
            public IDictionary<string, TemplateData> Templates { get; }
            public bool FoundErrorsInTemplates { get; }
        }

        private class TagCollection
        {
            private readonly IDictionary<string, MultiLanguageString> tagsById;
            private ILogger<XmlFileTemplateRepository> logger; 

            public TagCollection(ILogger<XmlFileTemplateRepository> logger) : this(null, logger)
            {
            }
            public TagCollection(IFileInfo tagsFile, ILogger<XmlFileTemplateRepository> logger)
            {
                if (tagsFile == null)
                {
                    tagsById = new Dictionary<string, MultiLanguageString>();
                }
                this.logger = logger;
                var xml = new XmlDocument();
                using var tagsFileStream = tagsFile.CreateReadStream();
                xml.Load(tagsFileStream);
                tagsById = xml.SelectNodes("/tags/tag").Cast<XmlElement>().ToDictionary(
                    e => e.GetAttribute("id") switch { null => throw new Exception("Each tag in the tags file should have a id"), var x => x},
                    e => ConvertMultiLanguageElement(e, "/tags/tag", logger)
                );
            }
            public MultiLanguageString GetTagById(string id)
            {
                tagsById.TryGetValue(id, out var result);
                return result;
            }
        }
    }

}
