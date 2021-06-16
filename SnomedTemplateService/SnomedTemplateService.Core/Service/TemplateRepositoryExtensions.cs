using SnomedTemplateService.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnomedTemplateService.Core.Service
{
    public static class TemplateRepositoryExtensions
    {
        public static IList<TemplateData> GetTemplates(this ITemplateRepository repository, Func<TemplateData, bool> filter)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            return repository.GetTemplates().Where(filter).ToList();
        }
    }
}
