using SnomedTemplateService.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnomedTemplateService.Core.Interfaces
{
    public interface ITemplateRepository
    {
        TemplateData GetById(string id);
        IList<TemplateData> GetTemplates();
        bool FoundErrorsInTemplates { get; }
    }
}
