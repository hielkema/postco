using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
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
        
        private readonly ImmutableDictionary<string, TemplateData> templates;
        private const string templatesCacheKey = "SNOMEDTEMPLATES";
        
        public XmlFileTemplateRepository(IMemoryCache memoryCache, IHostEnvironment hostEnvironment, IConfiguration configuration, ILogger<XmlFileTemplateRepository> logger)
        {
            if (!memoryCache.TryGetValue(templatesCacheKey, out templates))
            {
                var templateDirectoryPath = GetTemplateDirectoryPath(configuration);
                var templateDirectoryContents = GetTemplateDirectoryContents(templateDirectoryPath, hostEnvironment, logger);
                templates = ImmutableDictionary.ToImmutableDictionary(GetTemplateDictionary(templateDirectoryContents, logger));
                
                memoryCache.Set(templatesCacheKey, templates, hostEnvironment.ContentRootFileProvider.Watch($"{templateDirectoryPath}\\*\\*.xml"));
            }
        }

        public static void CheckTemplates(IHostEnvironment hostEnvironment, IConfiguration configuration, ILogger logger)
        {
            var templateDirectoryPath = GetTemplateDirectoryPath(configuration);
            GetTemplateDictionary(GetTemplateDirectoryContents(templateDirectoryPath, hostEnvironment, logger), logger);
        }

        private static string GetTemplateDirectoryPath(IConfiguration configuration)
        {
            return configuration["SnomedTemplatesDirectory"]?.TrimEnd('\\') ?? "SnomedTemplates";
        }

        private static IDirectoryContents GetTemplateDirectoryContents(string path, IHostEnvironment hostEnvironment, ILogger logger)
        {
            var result = hostEnvironment.ContentRootFileProvider.GetDirectoryContents(path);
            if (!result.Exists)
            {
                logger.LogError("The Templates Directory ({templateDirName}) doesn't exist.", path);
                throw new Exception($"The Templates Directory ({path}) doesn't exist."); 
            }
            return result;
        }

        private static Dictionary<string, TemplateData> GetTemplateDictionary(IDirectoryContents templateDirectory, ILogger logger)
        {
            var _templates = new Dictionary<string, TemplateData>();
            foreach (var subdir in templateDirectory.Where(c => c.IsDirectory))
            {
                try
                {
                    if (subdir.Name.Contains("_"))
                    {
                        throw new Exception($"The name of the template directory '{subdir.Name}' contains a underscore.");
                    }
                    using var subDirFileProvider = new PhysicalFileProvider(subdir.PhysicalPath);
                    foreach (
                        var file in subDirFileProvider.GetDirectoryContents("")
                        .Where(f => !f.IsDirectory && f.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        )
                    {
                        try
                        {
                            var key = $"{subdir.Name}_{Regex.Replace(file.Name, @"\.xml$", "", RegexOptions.IgnoreCase)}";
                            using var fileStream = file.CreateReadStream();
                            _templates[key] = GetTemplateData(key, fileStream);
                        }
                        catch (Exception e)
                        {
                            logger.LogWarning(e, "Error in template file {subDirName}\\{fileName}", subdir.Name, file.Name);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Error in template directory {subDirName}\\.", subdir.Name);
                }
            }
            return _templates;
        }

        public TemplateData GetById(string id)
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


        private static TemplateData GetTemplateData(string key, Stream stream)
        {
            var doc = new XmlDocument();
            doc.Load(stream);
            var templateNode = doc.SelectSingleNode("/template");
            var tags = doc.SelectNodes("/template/tags/tag").Cast<XmlElement>().Select(e => e.InnerText).ToList();

            var result = new TemplateData(
                key,
                templateNode.SelectSingleNode("time")?.InnerText?.Trim(),
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
    }    
}
