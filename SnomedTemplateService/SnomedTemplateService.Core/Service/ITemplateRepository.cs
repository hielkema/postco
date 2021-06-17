using SnomedTemplateService.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnomedTemplateService.Core.Service
{
    public interface ITemplateRepository
    {
        TemplateData GetTemplateById(string id);
        IList<TemplateData> GetTemplates();
        string GetTagName(string tagId, string preferredLanguage);
        string GetTagId(string tagName, string preferredLanguage);
        bool FoundErrorsInTemplates { get; }
    }
}
