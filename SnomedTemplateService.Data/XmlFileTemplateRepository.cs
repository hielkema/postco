using System;
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

        public string GetById(int id)
        {
            return sourceDoc.SelectSingleNode($"//template[id={id}]/etl").InnerText;
        }
    }
}
