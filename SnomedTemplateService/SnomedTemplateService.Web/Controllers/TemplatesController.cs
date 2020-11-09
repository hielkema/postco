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
using SnomedTemplateService.Util;
using SnomedTemplateService.Core.Domain;

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

            if (templateData == null)
            {
                return NotFound();
            }

            var parseResult = parseService.ParseExpressionTemplate(templateData.Etl);

            Subexpression rootExpression = null;

            var subExpression = parseResult.Subexpression.Handle(
                handleSubexpression: e => { rootExpression = e; return ValueTuple.Create(); },
                handleConceptSlot: s => throw new Exception("slots are not supported for the root expression"),
                handleExpressionSlot: s => throw new Exception("slots are not supported for the root expression")
                );

            var result = TemplateMetadataToJson(templateData);
            result["template"] = EtlSubexpressionToJson(rootExpression, templateData.SlotTitles, templateData.SlotDescriptions);

            return result;
        }

        public List<object> Get()
        {
            var templateRepository = new XmlFileTemplateRepository(_hostEnvironment.ContentRootFileProvider.GetFileInfo("expressionTemplates.xml"));
            return templateRepository.GetTemplates().Select(t => (object) TemplateMetadataToJson(t)).ToList();

        }

        private IDictionary<string, object> TemplateMetadataToJson(TemplateData templateData)
        {
            var result = new Dictionary<string, object>
            {
                ["id"] = templateData.Id,
                ["time"] = templateData.Time
            };
            if (templateData.Authors.Count != 0)
            {
                result["authors"] = templateData.Authors.Select(a => !string.IsNullOrEmpty(a.Contact) ? (object)new { name = a.Name, contact = a.Contact } : new { name = a.Name }).ToArray();
            }
            result["title"] = templateData.Title;
            if (!string.IsNullOrEmpty(templateData.Description))
            {
                result["description"] = templateData.Description;
            }
            result["snomedVersion"] = templateData.SnomedVersion;
            result["snomedBranch"] = templateData.SnomedBranch;
            if (!string.IsNullOrEmpty(templateData.StringFormat))
            {
                result["stringFormat"] = templateData.StringFormat;
            }
            return result;
        }

        private object EtlSubexpressionToJson(Subexpression subexpression, IDictionary<string,string> slotTitles, IDictionary<string, string> slotDescriptions)
        {
            var focusConceptsJson = new List<object>();  
            foreach (var fc in subexpression.FocusConcepts.Select(fi=>fi.focus))
            {
                fc.Handle(
                    handleConceptReference: c =>
                    {
                        focusConceptsJson.Add(GetPrecoordinatedConceptJson(c.SctId));
                        return ValueTuple.Create();
                    },
                    handleConceptSlot: s =>
                    {
                        focusConceptsJson.Add(GetConceptSlotJson(s.ExpressionConstraint));
                        return ValueTuple.Create();
                    },
                    handleExpressionSlot: s =>
                    {
                        focusConceptsJson.Add(GetConceptSlotJson(s.ExpressionConstraint));
                        return ValueTuple.Create();
                    }
                );
            }
            var groups = subexpression.Refinement.Handle(
                handleSetRefinement: attrSetRefinement =>
                {
                    if (attrSetRefinement.AttributeGroups.Count != 0) throw new Exception("Groups are not supported for a set refinement");
                    return new List<IList<(TemplateInformationSlot info, EtlAttribute attr)>>() { attrSetRefinement.AttributeSet.Attributes };
                },
                handleGroupRefinement: attrGroupRefinement =>
                {
                    return attrGroupRefinement.AttributeGroups.Select(a => a.group.Attributes);
                });

            var groupsListJson = new List<List<object>>();

            foreach(var grp in groups)
            {
                var groupJson = new List<object>();
                groupsListJson.Add(groupJson);
                foreach (var (info, attr) in grp)
                {
                    var attrNameSctId = attr.AttributeName.Handle(
                        handleConceptReference: c => c.SctId.ToString(),
                        handleConceptSlot: s => throw new Exception("Replacement slots are not supported for attribute names."),
                        handleExpressionSlot: s => throw new Exception("Replacement slots are not supported for attribute names.")
                    );
                    
                    object attrJson = attr.AttributeValue.Handle<object>(
                        handleSubexpressionOrSlot: e => e.Handle(
                            handleSubexpression: sub =>
                            {
                                var result = new Dictionary<string, object>
                                {
                                    ["attribute"] = attrNameSctId
                                };
                                if (sub.IsConceptReference)
                                {
                                    var concept = sub.GetConceptReference();
                                    result["title"] = concept.Term;
                                    result["value"] = GetPrecoordinatedConceptJson(concept.SctId);
                                }
                                else
                                {
                                    var infoSlotName = info?.SlotName;
                                    if (infoSlotName != null)
                                    {
                                        result["title"] = slotTitles.ContainsKey(infoSlotName) ?
                                            slotTitles[info?.SlotName] :
                                            info?.SlotName;
                                        if (slotDescriptions.ContainsKey(infoSlotName))
                                        {
                                            result["description"] = slotDescriptions[infoSlotName];
                                        }
                                    }
                                    result["template"] = EtlSubexpressionToJson(sub, slotTitles, slotDescriptions);
                                }
                                return result;
                            },
                            handleConceptSlot: s =>
                                HandleConceptOrExpressionSlotInAttributeValue(
                                    info,
                                    attrNameSctId,
                                    new FirstOf<ConceptReplacementSlot, ExpressionReplacementSlot>(s),
                                    slotTitles,
                                    slotDescriptions
                                    ),
                            handleExpressionSlot: s =>
                                HandleConceptOrExpressionSlotInAttributeValue(
                                    info,
                                    attrNameSctId,
                                    new SecondOf<ConceptReplacementSlot, ExpressionReplacementSlot>(s),
                                    slotTitles,
                                    slotDescriptions
                                    )
                        ),
                        handleStringOrSlot: s => throw new Exception("Concrete slots/values are not supported"),
                        handleIntOrSlot: s => throw new Exception("Concrete slots/values are not supported"),
                        handleDecimalOrSlot: s => throw new Exception("Concrete slots/values are not supported"),
                        handleBoolOrSlot: s => throw new Exception("Concrete slots/values are not supported")
                    );
                    groupJson.Add(attrJson);
                }
            }
            return new
            {
                focus = focusConceptsJson.ToArray(),
                groups = groupsListJson.ToArray().ToArray()
            };
        }

        private object HandleConceptOrExpressionSlotInAttributeValue(
            TemplateInformationSlot infoSlot,
            string attributeName,
            OneOf<ConceptReplacementSlot, ExpressionReplacementSlot> valueSlot,
            IDictionary<string, string> slotTitles,
            IDictionary<string, string> slotDescriptions
            )
        {
            var result = new Dictionary<string, object>
            {
                ["attribute"] = attributeName
            };

            var infoSlotName = infoSlot?.SlotName;
            var valueSlotName = valueSlot.Handle(c => c.SlotName, e=> e.SlotName);

            if (valueSlotName != null && slotTitles.ContainsKey(valueSlotName))
            {
                result["title"] = slotTitles[valueSlotName];
            }
            else if (infoSlotName != null && slotTitles.ContainsKey(infoSlotName))
            {
                result["title"] = slotTitles[infoSlotName];
            }
            else if (valueSlotName != null)
            {
                result["title"] = valueSlotName;
            }
            else if (infoSlotName != null)
            {
                result["title"] = infoSlotName;
            }

            if (valueSlotName != null && slotDescriptions.ContainsKey(valueSlotName))
            {
                result["description"] = slotDescriptions[valueSlotName];
            }
            else if (infoSlotName != null && slotDescriptions.ContainsKey(infoSlotName))
            {
                result["description"] = slotDescriptions[infoSlotName];
            }

            result["value"] = GetConceptSlotJson(valueSlot.Handle(c=>c.ExpressionConstraint, e=>e.ExpressionConstraint));
            return result;
        }

        private object GetConceptSlotJson(string constraint)
        {
            var result = new Dictionary<string, object>
            {
                ["type"] = "conceptSlot",
                ["constraint"] = constraint
            };
            return result;
        }

        private object GetPrecoordinatedConceptJson(ulong sctId)
        {
            var result = new Dictionary<string, object>
            {
                ["type"] = "precoordinatedConcept",
                ["conceptId"] = sctId.ToString()
            };
            return result;
        }
    }
}
