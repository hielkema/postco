using System;
using System.Collections.Generic;
using SnomedTemplateService.Core.Domain;

namespace SnomedTemplateService.Core.Service
{
    public interface ITemplateParserService
    {
        IEnumerable<EtlExpressionTemplate> All();
        EtlExpressionTemplate GetById(int i);
    }
    public class TemplateParserService : ITemplateParserService
    {
        public IEnumerable<EtlExpressionTemplate> All()
        {
            throw new NotImplementedException();
        }

        public EtlExpressionTemplate GetById(int i)
        {
            throw new NotImplementedException();
        }
    }
}
