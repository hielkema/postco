using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SnomedTemplateService.Parser;
using SnomedTemplateService.Core.Domain.Etl;
using SnomedTemplateService.Data;
using System.Dynamic;
using System.ComponentModel;

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
            try
            {
                var templateRepository = new XmlFileTemplateRepository(_hostEnvironment.ContentRootFileProvider.GetFileInfo("expressionTemplates.xml"));
                var parseService = new AntlrEtlParseService();

                var templateData = templateRepository.GetById(id);

                if (templateData == null)
                {
                    return NotFound();
                }

                var parseResult = parseService.ParseExpressionTemplate(templateData.Etl);

                var subExpression = parseResult.Subexpression;

                var rootNode = subExpression.Handle(
                    handleSubexpression: e => e,
                    handleConceptSlot: s => throw new Exception("slots are not supported for the root expression"),
                    handleExpressionSlot: s => throw new Exception("slots are not supported for the root expression")
                    );

                ConceptReference focusConcept;

                if (rootNode.FocusConcepts.Count == 1 && rootNode.FocusConcepts.Single().info.IsEmpty())
                {
                    focusConcept = rootNode.FocusConcepts.Single().focus.Handle(
                        handleConceptReference: c => c,
                        handleConceptSlot: s => throw new Exception("slots are not supported for focus concepts"),
                        handleExpressionSlot: s => throw new Exception("slots are not supported for focus concepts")
                        );
                }
                else
                {
                    throw new Exception("Only templates with exactly one focus concept are supported");
                }


                var attrInfos = rootNode.Refinement.Handle(
                    attrSetRefinement =>
                    {
                        if (attrSetRefinement.AttributeGroups.Count != 0) throw new Exception("Groups are not supported for a set refinement");
                        return attrSetRefinement.AttributeSet.Attributes;
                    },
                    attrGroupRefinement =>
                    {
                        if (attrGroupRefinement.AttributeGroups.Count > 1)
                        {
                            throw new Exception("Multiple groups are not supported.");
                        };
                        (var info, var group) = attrGroupRefinement.AttributeGroups.FirstOrDefault();
                        return group?.Attributes ?? new List<(TemplateInformationSlot, EtlAttribute)>();
                    });

                var attributes = attrInfos.Select(
                    attrInfo => (attrInfo.attr, optional: attrInfo.info.Cardinality.MinCardinality == 0)
                    );

                var group = attributes.Select<(EtlAttribute attr, bool optional), object>(
                                a =>
                                {
                                    dynamic result = new ExpandoObject();
                                    var attrName = a.attr.AttributeName.Handle(
                                        handleConceptReference: c => c.SctId.ToString(),
                                        handleConceptSlot: s => throw new Exception("Replacement slots are not supported for attribute names."),
                                        handleExpressionSlot: s => throw new Exception("Replacement slots are not supported for attribute names.")
                                    );
                                    var attrValue = a.attr.AttributeValue.Handle(
                                        handleSubexpressionOrSlot: e => e.Handle(
                                            handleSubexpression: sub =>
                                            {
                                                if (sub.IsConceptReference)
                                                {
                                                    var concept = sub.GetConceptReference();
                                                    return (
                                                       title: concept.Term,
                                                       description: null,
                                                       attribute: attrName,
                                                       value: concept.SctId.ToString()
                                                    );
                                                }
                                                else
                                                {
                                                    throw new Exception("Subexpressions are not supported");
                                                }
                                            },
                                            handleConceptSlot: s => (
                                                title: templateData.SlotTitles.ContainsKey(s.SlotName) ?
                                                    templateData.SlotTitles[s.SlotName] :
                                                    s.SlotName,
                                                description: templateData.SlotDescriptions.ContainsKey(s.SlotName) ?
                                                    templateData.SlotDescriptions[s.SlotName] :
                                                    null,
                                                attribute: attrName,
                                                value: s.ExpressionConstraint
                                            ),
                                            handleExpressionSlot: s => (
                                                title: templateData.SlotTitles.ContainsKey(s.SlotName) ?
                                                    templateData.SlotTitles[s.SlotName] :
                                                    s.SlotName,
                                                description: templateData.SlotDescriptions.ContainsKey(s.SlotName) ?
                                                    templateData.SlotDescriptions[s.SlotName] :
                                                    null,
                                                attribute: attrName,
                                                value: s.ExpressionConstraint
                                            )
                                        ),
                                        handleStringOrSlot: s => throw new Exception("Concrete slots/values are not supported"),
                                        handleIntOrSlot: s => throw new Exception("Concrete slots/values are not supported"),
                                        handleDecimalOrSlot: s => throw new Exception("Concrete slots/values are not supported"),
                                        handleBoolOrSlot: s => throw new Exception("Concrete slots/values are not supported")
                                    );
                                    result.title = attrValue.title;
                                    if (attrValue.description != null)
                                        result.description = attrValue.description;
                                    result.attribute = attrName;
                                    result.value = attrValue.value;

                                    return result;
                                }).ToArray();

                dynamic result = new ExpandoObject();

                result.id = templateData.Id;
                result.time = templateData.Time;
                if (templateData.Authors.Count != 0)
                {
                    result.authors = templateData.Authors.Select(a => !string.IsNullOrEmpty(a.Contact) ? (object)new { name = a.Name, contact = a.Contact } : new { name = a.Name }).ToArray();
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
                    root = focusConcept.SctId.ToString(),
                    groups = new object[][] {
                            group
                        }
                };

                return result;
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
