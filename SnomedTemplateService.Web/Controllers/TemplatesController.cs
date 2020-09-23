using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.IO;
using SnomedTemplateService.Parser.Generated;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using SnomedTemplateService.Parser;
using SnomedTemplateService.Core.Interfaces;
using SnomedTemplateService.Core.Domain;
using SnomedTemplateService.Data;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace SnomedTemplateService.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ILogger<TemplatesController> _logger;
        private readonly IHostEnvironment _hostEnvironment;


        public TemplatesController(ILogger<TemplatesController> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("{id}")]
        public object Get(int id)
        {
            var templateRepository = new XmlFileTemplateRepository(_hostEnvironment.ContentRootFileProvider.GetFileInfo("expressionTemplates.xml"));
            var parseService = new AntlrEtlParseService();
            
            var strTemplate = templateRepository.GetById(id);

            var parseResult = parseService.ParseExpressionTemplate(strTemplate);

            var subExpression = parseResult.SubExpression;
            
            var root = subExpression.FocusConcepts.First().Item2.ConceptReference.Handle(
                constReference  => constReference.SctId,
                slot => throw new Exception("slots are not supported for focus concepts"),
                slot => throw new Exception("slots are not supported for focus concepts")).ToString();

            var attrInfos = subExpression.Refinement.Handle(
                attrSetRefinement => {
                    if (attrSetRefinement.AttributeGroups.Count != 0) throw new Exception("Groups are not supported for a set refinement");
                    return attrSetRefinement.AttributeSet.Attributes;
                },
                attrGroupRefinement =>
                {
                    if (attrGroupRefinement.AttributeGroups.Count > 1) {
                        throw new Exception("Multiple groups are not supported.");
                    };
                    (var info, var group) = attrGroupRefinement.AttributeGroups.FirstOrDefault();
                    return group?.AttributeSet?.Attributes ?? new List<(EtlTemplateInformationSlot, EtlAttribute)>();
                });

            var attributes = attrInfos.Select(
                attrInfo => (attrInfo.attr, optional: attrInfo.info.Cardinality.MinCardinality == 0)
                );

            var group = attributes.Select(
                            a => a.attr.AttributeValue.Handle(
                                exp => exp.Value.Handle(
                                    sub => throw new Exception("Subexpressions are not supported"),
                                    conceptRef => conceptRef.ConceptReference.Handle<object>(
                                        concept => new { title = concept.Term, value = concept.SctId },
                                        conceptReplacement => new { title = conceptReplacement.SlotName, value = conceptReplacement.ExpressionConstraint },
                                        exprReplacement => throw new Exception("Expression replacement slots are not supported")
                                    )
                                ),
                                concrete => throw new Exception("Concrete slots/values are not supported")
                            )
                        ).ToArray();            

            return new
            {
                id,
                template = new
                {
                    root,
                    groups = new object[][] {
                        group    
                    }
                }
            };
        }
    }
}
