using SnomedTemplateService.Core.Domain.Etl;

namespace SnomedTemplateService.Core.Interfaces
{
    public interface IEtlParseService
    {
        ExpressionTemplate ParseExpressionTemplate(string template);
    }
}
