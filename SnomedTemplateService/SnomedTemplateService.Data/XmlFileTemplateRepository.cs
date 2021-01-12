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
        private static bool foundErrorsInTemplates = false;
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
                bool foundErrorsInEtl = false;
                if (!foundErrorsInXml)
                {
                    foundErrorsInEtl = templatesMutableDictionary.Aggregate(
                        foundErrorsInEtl,
                        (foundErrorsInPrecedingTemplates, kv) =>
                        {
                            var foundErrorInCurrentTemplate = false;
                            try
                            {
                                etlParser.ParseExpressionTemplate(kv.Value.Etl);
                            }
                            catch (Exception e)
                            {
                                foundErrorInCurrentTemplate = true;
                                logger.LogError(e, "The template with key={key} contains an ETL syntax error", kv.Key);
                            }
                            return foundErrorsInPrecedingTemplates || foundErrorInCurrentTemplate;
                        }
                        );
                }
                foundErrorsInTemplates = foundErrorsInXml || foundErrorsInEtl;
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
                                var match = Regex.Match(file.Name, @"^(.*?_(-?[1-9]\d*))\.xml$", RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    var key = $"{subdir.Name}_{match.Groups[1].Value}";
                                    using var fileStream = file.CreateReadStream();
                                    templates[key] = GetTemplateData(key, match.Groups[2].Value, fileStream);

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


        private TemplateData GetTemplateData(string key, string timestamp, Stream stream)
        {
            var doc = new XmlDocument();
            doc.Load(stream);
            var templateNode = doc.SelectSingleNode("/template");
            var tags = doc.SelectNodes("/template/tags/tag").Cast<XmlElement>().Select(e => e.InnerText).ToList();

            var result = new TemplateData(
                key,
                timestamp,
                templateNode.SelectSingleNode("snomedVersion")?.InnerText?.Trim(),
                templateNode.SelectSingleNode("snomedBranch")?.InnerText?.Trim(),
                templateNode.SelectSingleNode("etl")?.InnerText?.Trim(),
                tags
            )
            {
                Description = templateNode.SelectSingleNode("description")?.InnerText?.Trim(),
            };

            var title = templateNode.SelectSingleNode("title")?.InnerText?.Trim();
            if (title != null)
            {
                result.Title = title;
            }
            result.StringFormat = templateNode.SelectSingleNode("stringFormat")?.InnerText?.Trim();
            result.ItemTitles = templateNode.SelectNodes("items/item").Cast<XmlElement>().ToDictionary(e => e.GetAttribute("name"), e => e.SelectSingleNode("title")?.InnerText ?? e.GetAttribute("name"));
            result.ItemTitles = result.ItemTitles.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);
            result.ItemDescriptions = templateNode.SelectNodes("items/item").Cast<XmlElement>().ToDictionary(e => e.GetAttribute("name"), e => e.SelectSingleNode("description")?.InnerText);
            result.ItemDescriptions = result.ItemDescriptions.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);
            result.Authors = templateNode.SelectNodes("authors/author")
                .Cast<XmlElement>()
                .Select(a => new TemplateAuthor(a.SelectSingleNode("name")?.InnerText?.Trim())
                {
                    Contact = a.SelectSingleNode("contact")?.InnerText?.Trim()
                }).ToList();

            return result;
        }
        private class TemplateCollection
        {
            public TemplateCollection(IDictionary<string, TemplateData> templates, bool foundErrorsInTemplates)
            {
                Templates = ImmutableDictionary.ToImmutableDictionary(templates);
                FoundErrorsInTemplates = foundErrorsInTemplates;
            }
            public IDictionary<string, TemplateData> Templates { get; }
            public bool FoundErrorsInTemplates { get; }
        }
    }

}
