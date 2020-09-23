using SnomedTemplateService.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnomedTemplateService.Core.Interfaces
{
    public interface ITemplateRepository
    {
        string GetById(int id);
    }
}
