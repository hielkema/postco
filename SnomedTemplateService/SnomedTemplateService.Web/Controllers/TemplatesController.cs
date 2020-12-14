using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SnomedTemplateService.Core.Domain.Etl;
using SnomedTemplateService.Util;
using SnomedTemplateService.Core.Domain;
using Microsoft.AspNetCore.Cors;
using SnomedTemplateService.Core.Interfaces;

namespace SnomedTemplateService.Web.Controllers
{
    [EnableCors(Policies.CorsAllowAnyOrigin)]
    [ApiController]
    [Route("[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ILogger<TemplatesController> _logger;
        private readonly ITemplateRepository templateRepository;
        private readonly IEtlParseService parseService;

        public TemplatesController(
            ILogger<TemplatesController> logger,
            ITemplateRepository templateRepository,
            IEtlParseService parseService
        )
        {
            _logger = logger;
            this.templateRepository = templateRepository;
            this.parseService = parseService;
        }

        [HttpGet("{id}")]
        public object Get(string id)
        {
            try
            {
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
                var templateJson = EtlSubexpressionToJson(rootExpression, templateData.ItemTitles, templateData.ItemDescriptions, true);
                templateJson["definitionStatus"] = parseResult.DefinitionStatus.Handle(
                    lit => lit == DefinitionStatusEnum.subtypeOf ? "<<<" : "===",
                    slot => "slot");

                result["template"] = templateJson;

                return result;
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"An Exception occured in {nameof(TemplatesController.Get)}");
                throw;
            }
        }

        [HttpGet("")]
        public IEnumerable<object> List(string tag)
        {
            return templateRepository.GetTemplates().Select(t => TemplateMetadataToJson(t, tag)).Where(m => m != null).ToList();
        }

        private IDictionary<string, object> TemplateMetadataToJson(TemplateData templateData, string filterTag = null)
        {
            if (filterTag != null && !templateData.Tags.Contains(filterTag, StringComparer.InvariantCultureIgnoreCase))
            {
                return null;
            }
            
            var result = new Dictionary<string, object>
            {
                ["id"] = templateData.Id,
                ["time"] = templateData.TimeStamp
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
            result["tags"] = templateData.Tags;
            if (!string.IsNullOrEmpty(templateData.StringFormat))
            {
                result["stringFormat"] = templateData.StringFormat;
            }
            return result;
        }

        private IDictionary<string, object> EtlSubexpressionToJson(
            Subexpression subexpression, 
            IDictionary<string,string> itemTitles, IDictionary<string, string> itemDescriptions,
            bool isRootExpression = false)
        {
            var focusConceptsJson = new List<object>();
            foreach (var fc in subexpression.FocusConcepts)
            {
                var td = GetConceptOrSlotTitleAndDesc(fc.info, fc.focus, itemTitles, itemDescriptions);
                if (!isRootExpression && td.title==null)
                {
                    throw new Exception("Title is not specified for a focus-concept of a nested expression.");
                }
                fc.focus.Handle(
                    handleConceptReference: c =>
                    {
                        focusConceptsJson.Add(GetPrecoordinatedConceptJson(c.SctId, td.title, td.desc));
                        return ValueTuple.Create();
                    },
                    handleConceptSlot: s =>
                    {
                        focusConceptsJson.Add(GetConceptSlotJson(s.ExpressionConstraint, td.title, td.desc));
                        return ValueTuple.Create();
                    },
                    handleExpressionSlot: s =>
                    {
                        focusConceptsJson.Add(GetConceptSlotJson(s.ExpressionConstraint, td.title, td.desc));
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
                    var attrName = attr.AttributeName.Handle(
                        handleConceptReference: c => c,
                        handleConceptSlot: s => throw new Exception("Replacement slots are not supported for attribute names."),
                        handleExpressionSlot: s => throw new Exception("Replacement slots are not supported for attribute names.")
                    );

                    object attrJson = attr.AttributeValue.Handle(
                        handleSubexpressionOrSlot: e => e.Handle(
                            handleSubexpression: sub =>
                            {
                                var result = new Dictionary<string, object>
                                {
                                    ["attribute"] = attrName.SctId
                                };
                                var td = GetConceptOrSlotTitleAndDesc(info, attrName, itemTitles, itemDescriptions);
                                result["title"] = td.title ?? throw new Exception($"No title specified for {attrName.SctId}");
                                if (td.desc != null)
                                {
                                    result["description"] = td.desc;
                                }
                                if (sub.IsConceptReference)
                                {
                                    var concept = sub.GetConceptReference();
                                    result["value"] = GetPrecoordinatedConceptJson(concept.SctId);
                                }
                                else
                                {
                                    result["template"] = EtlSubexpressionToJson(sub, itemTitles, itemDescriptions);
                                }
                                return result;
                            },
                            handleConceptSlot: s =>
                                HandleConceptOrExpressionSlotInAttributeValue(
                                    info,
                                    attrName,
                                    new FirstOf<ConceptReplacementSlot, ExpressionReplacementSlot>(s),
                                    itemTitles,
                                    itemDescriptions
                                    ),
                            handleExpressionSlot: s =>
                                HandleConceptOrExpressionSlotInAttributeValue(
                                    info,
                                    attrName,
                                    new SecondOf<ConceptReplacementSlot, ExpressionReplacementSlot>(s),
                                    itemTitles,
                                    itemDescriptions
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
            return new Dictionary<string, object>
            {
                ["focus"] = focusConceptsJson.ToArray(),
                ["groups"] = groupsListJson.ToArray().ToArray()
            };
        }

        private object HandleConceptOrExpressionSlotInAttributeValue(
            TemplateInformationSlot infoSlot,
            ConceptReference attrName,
            OneOf<ConceptReplacementSlot, ExpressionReplacementSlot> valueSlot,
            IDictionary<string, string> itemTitles,
            IDictionary<string, string> itemDescriptions
            )
        {
            var td = GetConceptOrSlotTitleAndDesc(infoSlot, attrName, itemTitles, itemDescriptions);
            var result = new Dictionary<string, object>
            {
                ["attribute"] = attrName.SctId
            };
            
            result["title"] = td.title ?? throw new Exception($"No title specified for {attrName.SctId}");
            if (td.desc != null)
            {
                result["description"] = td.desc;
            }
            result["value"] = GetConceptSlotJson(valueSlot.Handle(c=>c.ExpressionConstraint, e=>e.ExpressionConstraint));
            return result;
        }

        private (string title, string desc) GetConceptOrSlotTitleAndDesc(
            TemplateInformationSlot info, IConceptReferenceOrSlot conceptOrSlot,
            IDictionary<string, string> titles, IDictionary<string, string> descriptions)
        {
            var name = info?.SlotName;
            string desc = null;
            string title = null;
            if (name != null)
            {
                titles.TryGetValue(name, out title);
                descriptions.TryGetValue(name, out desc);
            }
            title ??= conceptOrSlot.Handle(c => c.Term, cs => null, es => null);
            return (title, desc);
        }

        private object GetConceptSlotJson(string constraint, string title=null, string description=null)
        {
            var result = new Dictionary<string, object>
            {
                ["type"] = "conceptSlot",
                ["constraint"] = constraint
            };
            if (title != null)
            {
                result["title"] = title;
            }
            if (description != null)
            {
                result["description"] = description;
            }
            return result;
        }

        private object GetPrecoordinatedConceptJson(ulong sctId, string title = null, string description = null)
        {
            var result = new Dictionary<string, object>
            {
                ["type"] = "precoordinatedConcept",
                ["conceptId"] = sctId.ToString()
            };
            if (title != null)
            {
                result["title"] = title;
            }
            if (description != null)
            {
                result["description"] = description;
            }
            return result;
        }
    }
}
