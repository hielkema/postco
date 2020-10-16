using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SnomedTemplateService.Parser;
using SnomedTemplateService.Core.Domain;
using SnomedTemplateService.Data;
using System.Dynamic;

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
            
            var templateData = templateRepository.GetById(id);

            var parseResult = parseService.ParseExpressionTemplate(templateData.Etl);

            var subExpression = parseResult.SubExpression;

            var root = subExpression.FocusConcepts.First().focus.ConceptReference.Handle(
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

            var group = attributes.Select<(EtlAttribute attr, bool optional), object>(
                            a => {
                                dynamic result = new ExpandoObject();
                                var attrName = a.attr.AttributeName.ConceptReference.Handle(
                                    constRef => constRef.SctId.ToString(),
                                    conceptSlot => throw new Exception("Replacement slots are not supported for attribute names."),
                                    exprSlot => throw new Exception("Replacement slots are not supported for attribute names.")
                                );
                                var attrValue = a.attr.AttributeValue.Handle(
                                    exp => exp.Value.Handle(
                                        sub => throw new Exception("Subexpressions are not supported"),
                                        conceptRef => conceptRef.ConceptReference.Handle(
                                            concept => (
                                                title: concept.Term, 
                                                description: null,
                                                attribute: attrName, 
                                                value: concept.SctId.ToString()
                                            ),
                                            conceptReplacement => (
                                                title: templateData.SlotTitles.ContainsKey(conceptReplacement.SlotName) ? 
                                                    templateData.SlotTitles[conceptReplacement.SlotName] : 
                                                    conceptReplacement.SlotName,
                                                description: templateData.SlotDescriptions.ContainsKey(conceptReplacement.SlotName) ?
                                                    templateData.SlotDescriptions[conceptReplacement.SlotName] :
                                                    null,
                                                attribute: attrName, 
                                                value: conceptReplacement.ExpressionConstraint
                                            ),
                                            exprReplacement => throw new Exception("Expression replacement slots are not supported")
                                        )
                                    ),
                                    concrete => throw new Exception("Concrete slots/values are not supported")
                                );

                                result.title = attrValue.title;
                                if (attrValue.description != null)
                                    result.description = attrValue.description;
                                result.attribute = attrName;
                                result.value = attrValue.value;

                                return result;
                            }
                        ).ToArray();
            dynamic result = new ExpandoObject();

            result.id = templateData.Id;
            result.time = templateData.Time;
            if (templateData.Authors.Count != 0)
            {
                result.authors = templateData.Authors.Select(a => !string.IsNullOrEmpty(a.Contact) ? (object) new { name = a.Name, contact = a.Contact } : new { name = a.Name }).ToArray();
            }
            result.title = templateData.Title;
            if (!string.IsNullOrEmpty(templateData.Description))
            {
                result.description = templateData.Description;
            }
            result.snomedVersion = templateData.SnomedVersion;
            result.snomedBranch = templateData.SnomedBranch;
            
            result.template = new
            {
                root,
                groups = new object[][] {
                        group
                    }
            };

            return result;
        }
    }
}
