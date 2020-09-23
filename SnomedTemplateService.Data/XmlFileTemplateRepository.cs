using System;
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
            sourceDoc.Load(fileInfo.CreateReadStream());
        }
        public string GetById(int id)
        {
            return sourceDoc.SelectSingleNode($"//template[id={id}]/etl").InnerText;
        }
    }
}
