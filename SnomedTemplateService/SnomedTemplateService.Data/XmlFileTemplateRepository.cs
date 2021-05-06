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
using SnomedTemplateService.Core.Service;

namespace SnomedTemplateService.Data
{
    public class CachedTemplateRepository : ITemplateRepository
    {
        public CachedTemplateRepository(
            IEtlParseService etlParser,
            IMemoryCache memoryCache,
            IHostEnvironment hostEnvironment,
            IConfiguration configuration,
            ILogger<CachedTemplateRepository> logger,
            ILoggerFactory loggerFactory
        )
        {
            this.logger = logger;
            if (!memoryCache.TryGetValue(templatesCacheKey, out templateRepository))
            {
                logger.LogInformation("Refreshing templates-dictionary.");
                var templateDirectoryPath = GetTemplateDirectoryPath(configuration);
                var templateDirectoryContents = GetTemplateDirectoryContents(templateDirectoryPath, hostEnvironment);
                templateRepository = new XmlFileTemplateRepository(etlParser, templateDirectoryContents, loggerFactory.CreateLogger<XmlFileTemplateRepository>());

                memoryCache.Set(templatesCacheKey, templateRepository, hostEnvironment.ContentRootFileProvider.Watch($"{templateDirectoryPath}\\*\\*.xml"));
            }
        }

        private const string templatesCacheKey = "SNOMEDTEMPLATES";
        private readonly ILogger<CachedTemplateRepository> logger;
        private readonly ITemplateRepository templateRepository;

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

        public bool FoundErrorsInTemplates => templateRepository.FoundErrorsInTemplates;

        public TemplateData GetTemplateById(string id)
        {
            return templateRepository.GetTemplateById(id);
        }

        public IList<TemplateData> GetTemplates()
        {
            return templateRepository.GetTemplates();
        }

        public string GetTagName(string tagId, string preferredLanguage)
        {
            return templateRepository.GetTagName(tagId, preferredLanguage);
        }

        public string GetTagId(string tagName, string preferredLanguage)
        {
            return templateRepository.GetTagId(tagName, preferredLanguage);
        }
    }


    public class XmlFileTemplateRepository : ITemplateRepository
    {
        private readonly ImmutableDictionary<string, TemplateData> templates;
        private ImmutableDictionary<string, MultiLanguageString> tags;
        private readonly ILogger<XmlFileTemplateRepository> logger;
        private readonly IEtlParseService etlParser;
        private string defaultTagLanguage;

        public XmlFileTemplateRepository(
            IEtlParseService etlParser,
            IDirectoryContents templateDirectoryContents,
            ILogger<XmlFileTemplateRepository> logger
            )
        {
            this.etlParser = etlParser;
            this.logger = logger;

            var foundErrorsInXml = !InitTagData(templateDirectoryContents);
            foundErrorsInXml |= !GetTemplateDictionary(templateDirectoryContents, out var templatesMutableDictionary);

            var nrOfItemsBeforeCheckingEtl = templatesMutableDictionary.Count;
            templatesMutableDictionary = RemoveInvalidEtlTemplates(templatesMutableDictionary);
            FoundErrorsInTemplates = foundErrorsInXml || (nrOfItemsBeforeCheckingEtl != templatesMutableDictionary.Count);
            
            templates = templatesMutableDictionary.ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase);
        }

        public TemplateData GetTemplateById(string id)
        {
            if (templates.TryGetValue(id, out var result))
            {
                return result;
            }
            return null;
        }

        public IList<TemplateData> GetTemplates()
        {
            return templates.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
        }

        public bool FoundErrorsInTemplates
        {
            get;
        }

        public string GetTagName(string tagId, string preferredLanguage)
        {
            if (tagId == null)
            {
                return null;
            }
            tags.TryGetValue(tagId, out var tag);
            return tag[preferredLanguage] ?? tag[defaultTagLanguage];
        }
        
        public string GetTagId(string tagName, string preferredLanguage)
        {
            if (tagName == null)
            {
                return null;
            }
            foreach (var kv in tags)
            {
                if (kv.Value[preferredLanguage] == tagName) return kv.Key;
            }
            foreach(var kv in tags)
            {
                if (kv.Value[defaultTagLanguage] == tagName) return kv.Key;
            }
            return null;
        }

        private bool GetTemplateDictionary(
            IDirectoryContents templateDirectory, 
            out IDictionary<string, TemplateData> templates
            )
        {
            bool correct = true;
            templates = new Dictionary<string, TemplateData>(StringComparer.InvariantCultureIgnoreCase);
            
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
                                    templates[key] = GetTemplateData(key, match.Groups[2].Value, fileStream);
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

        private TemplateData GetTemplateData(string key, string timestamp, Stream stream)
        {
            var doc = new XmlDocument();
            doc.Load(stream);
            var templateNode = doc.SelectSingleNode("/template");

            var defaultLanguage = templateNode.Attributes["defaultLang"]?.Value;
            if (string.IsNullOrWhiteSpace(defaultLanguage))
            {
                throw new Exception("The defaultLang attribute of a template should be specified and non-empty.");
            }
            List<string> tagIds = new List<string>();

            try
            {
                tagIds = templateNode.SelectNodes("tags/tag").Cast<XmlElement>()
                    .Select(e => e.GetAttribute("id") switch {
                        var str when string.IsNullOrEmpty(str) => throw new Exception("Each tag reference in a template file should have an id attribute"),
                        var str => str
                    }).ToList();
            }
            catch
            {
                logger.LogError("error in tags collection of template");
                throw new ArgumentException("error in tags collection of template", nameof(stream));
            }
            foreach (var id in tagIds)
            {
                if (!tags.ContainsKey(id))
                {
                    logger.LogError("No tag exists with id={tagId}", id);
                    throw new ArgumentException($"No tag exists with id={id}", nameof(stream));
                }
            }
            if (tagIds.Count == 0)
            {
                logger.LogError("A template should have at least one tag.");
                throw new ArgumentException("A template should have at least one tag.", nameof(stream));
            }

            var title = ConvertMultiLanguageElement(templateNode.SelectSingleNode("title"), "/template/title", logger);

            if (!title.IsTranslatedFor(defaultLanguage))
            {
                throw new Exception("The title should have a non-empty translation for the default language");
            }

            var result = new TemplateData(
                key,
                timestamp,
                templateNode.SelectSingleNode("snomedVersion")?.InnerText?.Trim(),
                templateNode.SelectSingleNode("snomedBranch")?.InnerText?.Trim(),
                defaultLanguage,
                title,
                templateNode.SelectSingleNode("etl")?.InnerText?.Trim()
            )
            {
                Description = ConvertMultiLanguageElement(templateNode.SelectSingleNode("description"), "/template/description", logger),
                TagIds = tagIds
            };

            result.StringFormat = ConvertMultiLanguageElement(templateNode.SelectSingleNode("stringFormat"), "/template/stringFormat", logger);
            result.ItemData = templateNode.SelectNodes("items/item").Cast<XmlElement>()
                .ToDictionary(
                    e => e.GetAttribute("name"),
                    e => new TemplateData.Item(
                        ConvertMultiLanguageElement(e.SelectSingleNode("title"), "/template/items/item/title", logger),
                        ConvertMultiLanguageElement(e.SelectSingleNode("description"), "/template/item/description", logger)
                    ),
                    StringComparer.InvariantCultureIgnoreCase
                );

            if (result.ItemData.Values.Any(itm => !itm.Title.IsTranslatedFor(defaultLanguage)))
            {
                throw new Exception("All titles of the items in a template should have a non-empty translation for the default language");
            }

            result.Authors = templateNode.SelectNodes("authors/author")
                .Cast<XmlElement>()
                .Select(a => new TemplateData.Author(a.SelectSingleNode("name")?.InnerText?.Trim())
                {
                    Contact = a.SelectSingleNode("contact")?.InnerText?.Trim()
                }).ToList();

            return result;
        }
        
        private IDictionary<string, TemplateData> RemoveInvalidEtlTemplates(IDictionary<string, TemplateData> templates)
        {
            return templates.Where(
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
                    }
                    return !foundErrorsInEtl;
                }
            ).ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.InvariantCultureIgnoreCase);
        }

        private bool InitTagData(IDirectoryContents templateDirectoryContents)
        {
            try
            {
                var tagsFile = templateDirectoryContents.SingleOrDefault(f => string.Equals(f.Name, "tags.xml", StringComparison.OrdinalIgnoreCase));
                if (tagsFile == null)
                {
                    tags = ImmutableDictionary<string, MultiLanguageString>.Empty;
                    defaultTagLanguage = null;
                }
                else
                {
                    var xml = new XmlDocument();
                    using var tagsFileStream = tagsFile.CreateReadStream();
                    xml.Load(tagsFileStream);
                    tags = xml.SelectNodes("/tags/tag").Cast<XmlElement>().ToDictionary(
                        e => e.GetAttribute("id") switch
                        {
                            null => throw new Exception("Each tag must have a id"),
                            var x => x
                        },
                        e => ConvertMultiLanguageElement(e, "/tags/tag", logger)
                    ).ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase);
                    defaultTagLanguage = xml.DocumentElement.GetAttribute("defaultLang");
                    if (string.IsNullOrWhiteSpace(defaultTagLanguage)) throw new Exception("No default tag language specified");
                    if (tags.Values.Any(t=>!t.IsTranslatedFor(defaultTagLanguage)))
                    {
                        throw new Exception($"All tags should have a translation for the default language ({defaultTagLanguage})");
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occured while parsing tags file");
                return false;
            }
            return true;
        }

        private static MultiLanguageString ConvertMultiLanguageElement(XmlNode node, string path, ILogger<XmlFileTemplateRepository> logger)
        {
            path = path?.TrimEnd('/') ?? "";
            if (node == null)
            {
                return new MultiLanguageString();
            }

            var txtChilderen = node.SelectNodes("txt").Cast<XmlElement>();

            if (!txtChilderen.All(e => e.HasAttribute("lang") && e.GetAttribute("lang").Length > 0))
            {
                logger.LogError("Each element with path='{elementPath}/txt' must have a 'lang'-attribute.", path);
                throw new Exception($"Each element with path='{path}/txt' must have a 'lang'-attribute.");
            }

            if (!txtChilderen.All(e => !e.IsEmpty && (e.InnerText?.Trim().Length ?? 0) > 0 ))
            {
                logger.LogError("Each element with path='{elementPath}/txt' must be non-empty.", path);
                throw new Exception($"Each element with path='{path}/txt' must be non-empty.");
            }
            return new MultiLanguageString(
                txtChilderen.ToDictionary(e => e.GetAttribute("lang"), 
                e => e.InnerText.Trim(),
                StringComparer.InvariantCultureIgnoreCase
                )
            );
        }
    }
}
