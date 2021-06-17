using SnomedTemplateService.Core.Domain.Etl;

namespace SnomedTemplateService.Core.Service
{
    public interface IEtlParseService
    {
        ExpressionTemplate ParseExpressionTemplate(string template);
    }
}
