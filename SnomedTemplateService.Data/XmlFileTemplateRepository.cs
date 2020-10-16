using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Xml;
using Microsoft.Extensions.FileProviders;
using SnomedTemplateService.Core.Domain;
using SnomedTemplateService.Core.Interfaces;

namespace SnomedTemplateService.Data
{
    public class XmlFileTemplateRepository : ITemplateRepository
    {
        private XmlDocument sourceDoc; 
        public XmlFileTemplateRepository(IFileInfo fileInfo)
        {
            sourceDoc = new XmlDocument();
            using (var stream = fileInfo.CreateReadStream())
            {
                sourceDoc.Load(stream);
            }
        }

        public TemplateData GetById(int id)
        {
            var templateNode = sourceDoc.SelectSingleNode($"//template[@id={id}]");
            var result = new TemplateData(
                id,
                DateTime.Parse(templateNode.SelectSingleNode("time")?.InnerText?.Trim(), CultureInfo.InvariantCulture),
                templateNode.SelectSingleNode("snomedVersion")?.InnerText?.Trim(),
                templateNode.SelectSingleNode("snomedBranch")?.InnerText?.Trim(),
                templateNode.SelectSingleNode("etl")?.InnerText?.Trim()
            )
            {
                Description = templateNode.SelectSingleNode("description")?.InnerText?.Trim(),
            };
            var title = templateNode.SelectSingleNode("title")?.InnerText?.Trim();
            if (title!= null)
            {
                result.Title = title;
            }
            result.SlotTitles = templateNode.SelectNodes("slots/slot").Cast<XmlElement>().ToDictionary(e => e.GetAttribute("name"), e => e.SelectSingleNode("title")?.InnerText ?? e.GetAttribute("name"));
            result.SlotDescriptions = templateNode.SelectNodes("slots/slot").Cast<XmlElement>().ToDictionary(e => e.GetAttribute("name"), e => e.SelectSingleNode("description")?.InnerText);
            result.Authors = templateNode.SelectNodes("authors/author")
                .Cast<XmlElement>()
                .Select(a=>new TemplateAuthor(a.SelectSingleNode("name")?.InnerText?.Trim()) 
                {
                    Contact = a.SelectSingleNode("contact")?.InnerText?.Trim()
                }).ToList();

            return result;
             
        }
    }    
}
