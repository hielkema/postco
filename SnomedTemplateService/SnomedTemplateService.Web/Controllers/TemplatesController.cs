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
        public object Get(string id, string lang = "nl")
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

                var result = TemplateMetadataToJson(templateData, lang);
                var templateJson = EtlSubexpressionToJson(rootExpression, templateData.ItemData, lang, true);
                templateJson["definitionStatus"] = parseResult.DefinitionStatus.Handle(
                    lit => lit == DefinitionStatusEnum.subtypeOf ? "<<<" : "===",
                    slot => "slot");

                result["template"] = templateJson;

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An Exception occured in {nameof(TemplatesController.Get)}");
                throw;
            }
        }

        [HttpGet("")]
        public IEnumerable<object> List(string tag, string lang = "nl")
        {
            return templateRepository.GetTemplates().Select(t => TemplateMetadataToJson(t, lang, tag)).Where(m => m != null).ToList();
        }

        private IDictionary<string, object> TemplateMetadataToJson(TemplateData templateData, string lang, string filterTag = null)
        {
            lang = lang?.Trim() ?? "";

            if (lang.Length == 0)
            {
                throw new ArgumentNullException(nameof(lang));
            }
            if (filterTag != null && !templateData.Tags.Select(t=>t[lang]).Contains(filterTag, StringComparer.InvariantCultureIgnoreCase))
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
            result["title"] = templateData.Title[lang];
            if (!string.IsNullOrEmpty(templateData.Description[lang]))
            {
                result["description"] = templateData.Description[lang];
            }
            result["snomedVersion"] = templateData.SnomedVersion;
            result["snomedBranch"] = templateData.SnomedBranch;
            result["tags"] = templateData.Tags.Select(t=>t[lang]).Where(t=>!string.IsNullOrEmpty(t)).ToList();
            if (!string.IsNullOrEmpty(templateData.StringFormat[lang]))
            {
                result["stringFormat"] = templateData.StringFormat[lang];
            }
            return result;
        }

        private IDictionary<string, object> EtlSubexpressionToJson(
            Subexpression subexpression,
            IDictionary<string, TemplateData.Item> itemData,
            string lang,
            bool isRootExpression = false)
            {
            var focusConceptsJson = new List<object>();
            foreach (var (infoSlot, focusExpr) in subexpression.FocusConcepts)
            {
                var (title, desc) = GetConceptOrSlotTitleAndDesc(infoSlot, focusExpr, itemData, lang);
                if (!isRootExpression && title == null)
                {
                    throw new Exception("Title is not specified for a focus-concept of a nested expression.");
                }
                focusExpr.Handle(
                    handleConceptReference: c =>
                    {
                        focusConceptsJson.Add(GetPrecoordinatedConceptJson(c.SctId, title, desc));
                        return ValueTuple.Create();
                    },
                    handleConceptSlot: s =>
                    {
                        focusConceptsJson.Add(GetConceptSlotJson(s.ExpressionConstraint, title, desc));
                        return ValueTuple.Create();
                    },
                    handleExpressionSlot: s =>
                    {
                        focusConceptsJson.Add(GetConceptSlotJson(s.ExpressionConstraint, title, desc));
                        return ValueTuple.Create();
                    }
                );
            }
            var groups = subexpression.Refinement.Handle(
                handleSetRefinement: attrSetRefinement =>
                {
                    IEnumerable<IList<(TemplateInformationSlot info, EtlAttribute attr)>> singleAttributeGroups = attrSetRefinement.AttributeSet.Attributes.Select(
                        a => Enumerable.Repeat(a, 1).ToList());
                    return singleAttributeGroups.Concat(attrSetRefinement.AttributeGroups.Select(a => a.group.Attributes));
                },
                handleGroupRefinement: attrGroupRefinement => attrGroupRefinement.AttributeGroups.Select(a => a.group.Attributes)
            );

            var groupsListJson = new List<List<object>>();

            foreach (var grp in groups)
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
                                var (attrTitle, attrDesc) = GetConceptOrSlotTitleAndDesc(info, attrName, itemData, lang);
                                result["title"] = attrTitle ?? throw new Exception($"No title specified for {attrName.SctId}");
                                if (attrDesc != null)
                                {
                                    result["description"] = attrDesc;
                                }
                                result["cardinality"] = new
                                {
                                    min = info?.Cardinality?.MinCardinality ?? 1,
                                    max = info?.Cardinality?.MaxCardinality
                                };
                                if (sub.IsConceptReference)
                                {
                                    var concept = sub.GetConceptReference();
                                    result["value"] = GetPrecoordinatedConceptJson(concept.SctId);
                                }
                                else
                                {
                                    result["template"] = EtlSubexpressionToJson(sub, itemData, lang);
                                }
                                return result;
                            },
                            handleConceptSlot: s =>
                                HandleConceptOrExpressionSlotInAttributeValue(
                                    info,
                                    attrName,
                                    new FirstOf<ConceptReplacementSlot, ExpressionReplacementSlot>(s),
                                    itemData,
                                    lang
                                    ),
                            handleExpressionSlot: s =>
                                HandleConceptOrExpressionSlotInAttributeValue(
                                    info,
                                    attrName,
                                    new SecondOf<ConceptReplacementSlot, ExpressionReplacementSlot>(s),
                                    itemData,
                                    lang
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
            IDictionary<string, TemplateData.Item> itemData,
            string lang
            )
       {
            var (attrTitle, attrDesc) = GetConceptOrSlotTitleAndDesc(infoSlot, attrName, itemData, lang);
            var result = new Dictionary<string, object>
            {
                ["attribute"] = attrName.SctId
            };
            
            result["title"] = attrTitle ?? throw new Exception($"No title specified for {attrName.SctId}");
            if (attrDesc != null)
            {
                result["description"] = attrDesc;
            }
            result["cardinality"] = new
            {
                min = infoSlot?.Cardinality?.MinCardinality ?? 1,
                max = infoSlot?.Cardinality?.MaxCardinality
            };
            result["value"] = GetConceptSlotJson(valueSlot.Handle(c=>c.ExpressionConstraint, e=>e.ExpressionConstraint));
            return result;
        }
        private (string title, string desc) GetConceptOrSlotTitleAndDesc(
            TemplateInformationSlot info, IConceptReferenceOrSlot conceptOrSlot,
            IDictionary<string, TemplateData.Item> itemData,
            string lang)
        {
            var name = info?.SlotName;
            TemplateData.Item currentItem = null;
            if (name != null)
            {
                itemData.TryGetValue(name, out currentItem);
            }

            var title = currentItem?.Title[lang];
            var desc = currentItem?.Description[lang];
            title ??= conceptOrSlot.Handle(c => c?.Term, cs => null, es => null);
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
