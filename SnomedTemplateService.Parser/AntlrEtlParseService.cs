using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SnomedTemplateService.Core.Domain;
using SnomedTemplateService.Core.Exceptions;
using SnomedTemplateService.Core.Interfaces;
using SnomedTemplateService.Parser.Generated;
using SnomedTemplateService.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static SnomedTemplateService.Parser.Generated.ExpressionTemplateParser;

namespace SnomedTemplateService.Parser
{
    public class AntlrEtlParseService : IEtlParseService
    {
        public EtlExpressionTemplate ParseExpressionTemplate(string s)
        {
            var lexer = new ExpressionTemplateLexer(new AntlrInputStream(s));
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();
            var parser = new ExpressionTemplateParser(tokens)
            {
                ErrorHandler = new BailErrorStrategy()
            };
            var parseContext = parser.parse();
            ExpressiontemplateContext template;
            if (parseContext == null)
            {
                throw new ParserException();
            }
            else
            {
                template = parseContext.expressiontemplate();
            }
            if (template == null)
            {
                throw new ParserException();
            }
            return ConvertExpressiontemplate(template);
        }

        private EtlExpressionTemplate ConvertExpressiontemplate(ExpressiontemplateContext context)
        {
            var constDefStatus = context.definitionstatus();
            var tokenSlot = context.tokenreplacementslot();
            var subExpression = context.subexpression();

            OneOf<DefinitionStatusEnum, EtlTokenReplacementSlot> defStatus;
            if ((new object[] { constDefStatus, tokenSlot }).Count(s => s != null) > 1)
            {
                throw new ParserException();
            }
            if (constDefStatus != null)
            {
                defStatus = new FirstOf<DefinitionStatusEnum, EtlTokenReplacementSlot>(ConvertDefinitionstatus(constDefStatus));
            }
            else if (tokenSlot != null)
            {
                defStatus = new SecondOf<DefinitionStatusEnum, EtlTokenReplacementSlot>(ConvertTokenreplacementslot(tokenSlot));
            }
            else
            {
                throw new ParserException();
            }
            return subExpression switch 
            {
                null => throw new ParserException(), 
                var su => new EtlExpressionTemplate(defStatus, ConvertSubexpression(su)) 
            };
        }

        private EtlSubExpression ConvertSubexpression(SubexpressionContext context)
        {
            return new EtlSubExpression(
                ConvertFocusconcept(context.focusconcept()),
                context.refinement() switch
                {
                    null => null,
                    var r => ConvertRefinement(r)
                });
        }

        private DefinitionStatusEnum ConvertDefinitionstatus(DefinitionstatusContext context)
        {
            switch (context.GetText())
            {
                case "===":
                    return DefinitionStatusEnum.equivalentTo;

                case "<<<":
                    return DefinitionStatusEnum.subtypeOf;

                default:
                    throw new ParserException();
            }
        }

        private IList<(EtlTemplateInformationSlot info, EtlFocusConcept focusConcept)> ConvertFocusconcept(FocusconceptContext context)
        {
            var conceptReferences = new List<(TemplateinformationslotContext info, ConceptreferenceContext concept)>();
            TemplateinformationslotContext currentInformationSlot = null;
            foreach (var child in context.children)
            {
                switch (child)
                {
                    case null:
                        throw new ParserException();
                    case TemplateinformationslotContext ti:
                        if (currentInformationSlot != null)
                        {
                            throw new ParserException();
                        }
                        currentInformationSlot = ti;
                        break;
                    case ConceptreferenceContext cr:
                        conceptReferences.Add((currentInformationSlot, cr));
                        currentInformationSlot = null;
                        break;
                    default:
                        throw new ParserException();
                }
            }
            if (currentInformationSlot != null)
            {
                throw new ParserException();
            }
            return conceptReferences.Select(p =>
                (
                p.info switch { null => null, var s => ConvertTemplateinformationslot(s) },
                ConvertConceptreference(p.concept) switch { null => throw new ParserException(), var cr => new EtlFocusConcept(cr) }
                )
            ).ToList();
        }

        private EtlConceptReference ConvertConceptreference(ConceptreferenceContext context)
        {
            var conceptSlotContext = context.conceptreplacementslot();
            var expressionSlotContext = context.expressionreplacementslot();
            var conceptIdContext = context.conceptid();
            var termContext = context.term();

            EtlConceptReplacementSlot conceptReplacementSlot = null;
            EtlExpressionReplacementSlot expressionReplacementSlot = null;
            ulong conceptId = 0;
            string term = null;
            if ((new object[] { conceptSlotContext, expressionSlotContext, conceptIdContext }).Count(c => c != null) != 1)
            {
                throw new ParserException();
            }
            if (conceptSlotContext != null)
            {
                conceptReplacementSlot = ConvertConceptreplacementslot(conceptSlotContext);
            }
            if (expressionSlotContext != null)
            {
                expressionReplacementSlot = ConvertExpressionreplacementslot(expressionSlotContext);
            }
            if (conceptIdContext != null)
            {
                conceptId = ConvertConceptid(conceptIdContext);
                if (termContext != null)
                {
                    term = ConvertTerm(termContext);
                }
            }

            if (conceptReplacementSlot != null)
            {
                return new EtlConceptReference(new SecondOf<ConceptReference, EtlConceptReplacementSlot, EtlExpressionReplacementSlot>(conceptReplacementSlot));
            }
            if (expressionReplacementSlot != null)
            {
                return new EtlConceptReference(new ThirdOf<ConceptReference, EtlConceptReplacementSlot, EtlExpressionReplacementSlot>(expressionReplacementSlot));
            }
            ConceptReference conceptReference = new ConceptReference(conceptId, term);
            return new EtlConceptReference(new FirstOf<ConceptReference, EtlConceptReplacementSlot, EtlExpressionReplacementSlot>(conceptReference));
        }

        private ulong ConvertConceptid(ConceptidContext context)
        {
            return ulong.Parse(context.GetText());
        }

        private string ConvertTerm(TermContext context)
        {
            return context.GetText();
        }

        private OneOf<EtlAttributeSetRefinement, EtlAttributeGroupRefinement> ConvertRefinement(RefinementContext context)
        {
            var attributesetContext = context.attributeset();

            if (attributesetContext != null)
            {
                return new FirstOf<EtlAttributeSetRefinement, EtlAttributeGroupRefinement>(
                    new EtlAttributeSetRefinement(
                        ConvertAttributeset(attributesetContext),
                        context.attributegroup().Select(ag1 => ag1 switch
                        {
                            null => throw new ParserException(),
                            var ag2 => ConvertAttributegroup(ag2)
                        }
                        ).ToList()
                    )
                );
            }
            else
            {
                return new SecondOf<EtlAttributeSetRefinement, EtlAttributeGroupRefinement>(
                    new EtlAttributeGroupRefinement(
                        context.attributegroup().Select(ag1 => ag1 switch
                        {
                            null => throw new ParserException(),
                            var ag2 => ConvertAttributegroup(ag2)
                        }
                        ).ToList()
                    )
                );
            }
        }

        private (EtlTemplateInformationSlot info, EtlAttributeGroup group) ConvertAttributegroup(AttributegroupContext context)
        {
            return (context.templateinformationslot() switch { null => null, var ti => ConvertTemplateinformationslot(ti) },
            new EtlAttributeGroup(ConvertAttributeset(context.attributeset())));
        }

        private EtlAttributeSet ConvertAttributeset(AttributesetContext context)
        {
            return new EtlAttributeSet(context.attribute().Select(
                attr => attr switch
                {
                    null => throw new ParserException(),
                    var a => ConvertAttribute(a)
                }
            ).ToList());

        }

        private (EtlTemplateInformationSlot info, EtlAttribute attr) ConvertAttribute(AttributeContext context)
        {
            return (
                context.templateinformationslot() switch
                {
                    null => null,
                    var ti => ConvertTemplateinformationslot(ti)
                },
                new EtlAttribute(
                    context.attributename() switch
                    {
                        null => throw new ParserException(),
                        var attrName => ConvertAttributename(attrName)
                    },
                    context.attributevalue() switch
                    {
                        null => throw new ParserException(),
                        var attrValue => ConvertAttributevalue(attrValue)
                    }
                )
            );
        }

        private EtlAttributeName ConvertAttributename(AttributenameContext context)
        {
            return new EtlAttributeName(ConvertConceptreference(context.conceptreference()));
        }

        private OneOf<EtlExpressionValue, EtlConcreteValue> ConvertAttributevalue(AttributevalueContext context)
        {
            var expressionvalueContext = context.expressionvalue();
            var stringvalueContext = context.stringvalue();
            var numericvalueContext = context.numericvalue();
            var booleanvalueContext = context.booleanvalue();
            var concretevalueslotContext = context.concretevaluereplacementslot();
            OneOf<int, EtlIntReplacementSlot, decimal, EtlDecimalReplacementSlot, string, EtlStringReplacementSlot, bool, EtlBooleanReplacementSlot> switchConcreteValue = null;
            if ((new object[] { expressionvalueContext, stringvalueContext, numericvalueContext,
                booleanvalueContext, concretevalueslotContext}).Count(c => c != null) != 1)
            {
                throw new ParserException();
            }
            if (expressionvalueContext != null)
            {
                return new FirstOf<EtlExpressionValue, EtlConcreteValue>(ConvertExpressionvalue(expressionvalueContext));
            }
            else if (numericvalueContext != null)
            {
                var numericValue = ConvertNumericvalue(numericvalueContext);
                switchConcreteValue = numericValue.Handle<
                    OneOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>
                    >(
                    i => new FirstOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>(i),
                    d => new ThirdOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>(d)
                        );
            }
            else if (stringvalueContext != null)
            {
                var stringValue = ConvertStringvalue(stringvalueContext);
                switchConcreteValue =
                    new FifthOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>
                    (stringValue);
            }
            else if (booleanvalueContext != null)
            {
                var booleanValue = ConvertBooleanvalue(booleanvalueContext);
                switchConcreteValue =
                 new SeventhOf<int, EtlIntReplacementSlot,
                     decimal, EtlDecimalReplacementSlot,
                     string, EtlStringReplacementSlot,
                     bool, EtlBooleanReplacementSlot>
                 (booleanValue);
            }
            else if (concretevalueslotContext != null)
            {
                var concreteValueSlot = ConvertConcretevaluereplacementslot(concretevalueslotContext);
                switchConcreteValue =
                    concreteValueSlot.Handle<
                    OneOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>
                    >(
                        strslot => new SixthOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>(strslot),
                        intslot => new SecondOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>(intslot),
                        decslot => new FourthOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>(decslot),
                        boolslot => new EighthOf<int, EtlIntReplacementSlot,
                        decimal, EtlDecimalReplacementSlot,
                        string, EtlStringReplacementSlot,
                        bool, EtlBooleanReplacementSlot>(boolslot)
                    );
            }
            else
            {
                throw new ParserException();
            }
            return new SecondOf<EtlExpressionValue, EtlConcreteValue>(new EtlConcreteValue(switchConcreteValue));
        }

        private EtlExpressionValue ConvertExpressionvalue(ExpressionvalueContext context)
        {
            var conceptRef = context.conceptreference();
            var subExp = context.subexpression();
            if ((new object[] { conceptRef, subExp }).Where(x => x != null).Count() != 1)
            {
                throw new ParserException();
            }
            if (conceptRef != null)
            {
                return new EtlExpressionValue(new SecondOf<EtlSubExpression, EtlConceptReference>(ConvertConceptreference(conceptRef)));
            }
            else if (subExp != null)
            {
                return new EtlExpressionValue(new FirstOf<EtlSubExpression, EtlConceptReference>(ConvertSubexpression(subExp)));
            }
            else
            {
                throw new ParserException();
            }
        }

        private string ConvertStringvalue(StringvalueContext context)
        {
            return Regex.Replace(context.GetText(), @"\\([\""])", "$1");
        }

        private OneOf<int, decimal> ConvertNumericvalue(NumericvalueContext context)
        {
            var sign = context.DASH() != null ? -1 : 1; 
            if ((new object[] { context.decimalvalue(), context.integervalue() }).Count(c => c != null) != 1)
            {
                throw new ParserException();
            }
            else if (context.integervalue() != null)
            {
                return new FirstOf<int, decimal>(sign * ConvertIntegervalue(context.integervalue()));
            }
            else if (context.decimalvalue() != null)
            {
                return new SecondOf<int, decimal>(sign * ConvertDecimalvalue(context.decimalvalue()));
            }
            throw new ParserException();
        }

        private int ConvertIntegervalue(IntegervalueContext context)
        {
            return int.Parse(context.GetText());
        }

        private decimal ConvertDecimalvalue(DecimalvalueContext context)
        {
            return decimal.Parse(context.GetText());
        }

        private bool ConvertBooleanvalue(BooleanvalueContext context)
        {
            return bool.Parse(context.GetText());
        }

        private EtlConceptReplacementSlot ConvertConceptreplacementslot(ConceptreplacementslotContext context)
        {
            var slotName = context.slotname() switch { null => null, var s => ConvertSlotname(s) };
            var expressionConstraint = context.conceptreplacement().slotexpressionconstraint() switch { null => null, var e => ConvertSlotexpressionconstraint(e) };
            return new EtlConceptReplacementSlot(expressionConstraint, slotName);
        }

        private EtlExpressionReplacementSlot ConvertExpressionreplacementslot(ExpressionreplacementslotContext context)
        {
            var slotName = context.slotname() switch { null => null, var s => ConvertSlotname(s) };
            var expressionConstraint = context.expressionreplacement().slotexpressionconstraint() switch { null => null, var e => ConvertSlotexpressionconstraint(e) };
            return new EtlExpressionReplacementSlot(expressionConstraint, slotName);
        }

        private EtlTokenReplacementSlot ConvertTokenreplacementslot(TokenreplacementslotContext context)
        {
            var slotName = context.slotname() switch { null => null, var s => ConvertSlotname(s) };
            return new EtlTokenReplacementSlot(ConvertSlottokenset(context.tokenreplacement().slottokenset()), slotName);
        }

        private OneOf<EtlStringReplacementSlot, EtlIntReplacementSlot, EtlDecimalReplacementSlot, EtlBooleanReplacementSlot>
            ConvertConcretevaluereplacementslot(ConcretevaluereplacementslotContext context)
        {
            var concreteValueReplacement = context.concretevaluereplacement();
            if (concreteValueReplacement == null)
            {
                throw new ParserException();
            }
            var stringReplacement = concreteValueReplacement.stringreplacement();
            var intReplacement = concreteValueReplacement.integerreplacement();
            var decimalReplacement = concreteValueReplacement.decimalreplacement();
            var boolReplacement = concreteValueReplacement.booleanreplacement();
            var slotName = ConvertSlotname(context.slotname());
            if ((new object[] { stringReplacement, intReplacement, decimalReplacement, boolReplacement }).Count(c => c != null) != 1)
            {
                throw new ParserException();
            }
            if (stringReplacement != null)
            {
                return new FirstOf<EtlStringReplacementSlot, EtlIntReplacementSlot, EtlDecimalReplacementSlot, EtlBooleanReplacementSlot>(
                    new EtlStringReplacementSlot(
                        stringReplacement.slotstringset() switch
                        {
                            null => throw new ParserException(),
                            var s => ConvertSlotstringset(s)
                        }, 
                        slotName
                        )
                    );
            }
            else if (intReplacement != null)
            {
                var r = new EtlIntReplacementSlot(slotName);
                var integerReplacements = (intReplacement.slotintegerset() switch
                {
                    null => throw new ParserException(),
                    var i => ConvertSlotintegerset(i)
                }).ToList();

                foreach (var ir in integerReplacements)
                {
                    ir.Handle(i => { r.Values.Add(i); return ValueTuple.Create(); }, range => { r.Ranges.Add(range); return ValueTuple.Create(); });
                }

                return new SecondOf<EtlStringReplacementSlot, EtlIntReplacementSlot, EtlDecimalReplacementSlot, EtlBooleanReplacementSlot>(r);
            }
            else if (decimalReplacement != null)
            {
                var r = new EtlDecimalReplacementSlot(slotName);
                var decimalReplacements = (decimalReplacement.slotdecimalset() switch
                {
                    null => throw new ParserException(),
                    var d => ConvertSlotdecimalset(d)
                }).ToList();

                foreach (var dr in decimalReplacements)
                {
                    dr.Handle(d => { r.Values.Add(d); return ValueTuple.Create(); }, range => { r.Ranges.Add(range); return ValueTuple.Create(); });
                }

                return new ThirdOf<EtlStringReplacementSlot, EtlIntReplacementSlot, EtlDecimalReplacementSlot, EtlBooleanReplacementSlot>(r);
            }
            else if (boolReplacement != null)
            {
                return new FourthOf<EtlStringReplacementSlot, EtlIntReplacementSlot, EtlDecimalReplacementSlot, EtlBooleanReplacementSlot>(
                    new EtlBooleanReplacementSlot(
                        boolReplacement.slotbooleanset() switch
                        {
                            null => throw new ParserException(),
                            var b => ConvertSlotbooleanset(b)
                        },
                        slotName)
                    );
            }
            throw new ParserException();
        }

        private ICollection<SlotToken> ConvertSlottokenset(SlottokensetContext context)
        {
            return Regex.Replace(context.GetText().ToLowerInvariant(), @"\s+", " ") switch
            {
                "=== <<<" => new List<SlotToken>() { SlotToken.definitionstatus },
                "<<< ===" => new List<SlotToken>() { SlotToken.definitionstatus },
                _ => throw new ParserException("")
            };
        }

        private ICollection<string> ConvertSlotstringset(SlotstringsetContext context)
        {
            return context.slotstring().Select(slotstr => ConvertSlotstring(slotstr)).ToList();
        }

        private ICollection<OneOf<int, EtlIntReplacementSlot.Range>> ConvertSlotintegerset(SlotintegersetContext context)
        {
            return context.children.Select<IParseTree, OneOf<int, EtlIntReplacementSlot.Range>>(
                c => c switch
                {
                    SlotintegervalueContext iv => new FirstOf<int, EtlIntReplacementSlot.Range>(ConvertSlotintegervalue(iv)),
                    SlotintegerrangeContext ir => new SecondOf<int, EtlIntReplacementSlot.Range>(ConvertSlotintegerrange(ir)),
                    _ => throw new ParserException()
                }
            ).ToList();
        }

        private ICollection<OneOf<decimal, EtlDecimalReplacementSlot.Range>> ConvertSlotdecimalset(SlotdecimalsetContext context)
        {
            return context.children.Select<IParseTree, OneOf<decimal, EtlDecimalReplacementSlot.Range>>(
                c => c switch
                {
                    SlotdecimalvalueContext dv => new FirstOf<decimal, EtlDecimalReplacementSlot.Range>(ConvertSlotdecimalvalue(dv)),
                    SlotdecimalrangeContext dr => new SecondOf<decimal, EtlDecimalReplacementSlot.Range>(ConvertSlotdecimalrange(dr)),
                    _ => throw new ParserException()
                }
            ).ToList();
        }

        private ICollection<bool> ConvertSlotbooleanset(SlotbooleansetContext context)
        {
            return context.slotbooleanvalue().Select(slotbool => ConvertSlotbooleanvalue(slotbool)).ToList();
        }

        private EtlIntReplacementSlot.Range ConvertSlotintegerrange(SlotintegerrangeContext context)
        {

            var minimum = context.slotintegerminimum() switch
            {
                null => (int?)null,
                var m => ConvertSlotintegervalue(m.slotintegervalue())
            };

            var maximum = context.slotintegermaximum() switch
            {
                null => (int?)null,
                var m => ConvertSlotintegervalue(m.slotintegervalue())
            };
            bool? exclusiveMinimum = null;
            bool? exclusiveMaximum = null;

            if (minimum != null)
            {
                exclusiveMinimum = context.slotintegerminimum() switch
                {
                    null => false,
                    var im => im.exclusiveminimum() != null
                };
            }

            if (maximum != null)
            {
                exclusiveMaximum = context.slotintegermaximum() switch
                {
                    null => false,
                    var im => im.exclusivemaximum() != null
                };
            }

            return new EtlIntReplacementSlot.Range(exclusiveMinimum, minimum, exclusiveMaximum, maximum);
        }

        private int ConvertSlotintegervalue(SlotintegervalueContext context)
        {
            return int.Parse(context.GetText().TrimStart(new[] { '#' }));
        }

        private EtlDecimalReplacementSlot.Range ConvertSlotdecimalrange(SlotdecimalrangeContext context)
        {
            var minimum = context.slotdecimalminimum() switch
            {
                null => (decimal?)null,
                var m => ConvertSlotdecimalvalue(m.slotdecimalvalue())
            };

            var maximum = context.slotdecimalmaximum() switch
            {
                null => (decimal?)null,
                var m => ConvertSlotdecimalvalue(m.slotdecimalvalue())
            };

            bool? exclusiveMinimum = null;
            bool? exclusiveMaximum = null;

            if (minimum != null)
            {
                exclusiveMinimum = context.slotdecimalminimum() switch
                {
                    null => false,
                    var im => im.exclusiveminimum() != null
                };
            }

            if (maximum != null)
            {
                exclusiveMaximum = context.slotdecimalmaximum() switch
                {
                    null => false,
                    var im => im.exclusivemaximum() != null
                };
            }

            return new EtlDecimalReplacementSlot.Range(exclusiveMinimum, minimum, exclusiveMaximum, maximum);
        }

        private decimal ConvertSlotdecimalvalue(SlotdecimalvalueContext context)
        {
            return decimal.Parse(context.GetText().TrimStart(new[] { '#' }));
        }

        private bool ConvertSlotbooleanvalue(SlotbooleanvalueContext context)
        {
            return bool.Parse(context.GetText());
        }

        private string ConvertSlotname(SlotnameContext context)
        {
            return context.nonquotestringvalue() switch { null => null, var x => ConvertNonquotestringvalue(x) } ?? ConvertSlotstring(context.slotstring());
        }

        private string ConvertSlotstring(SlotstringContext context)
        {
            return Regex.Replace(context.slotstringvalue().GetText() switch { var x => x.Substring(1, x.Length - 2) }, @"\\([\""])", "$1");
        }

        private string ConvertNonquotestringvalue(NonquotestringvalueContext context)
        {
            return context.GetText();
        }

        private EtlTemplateInformationSlot ConvertTemplateinformationslot(TemplateinformationslotContext context)
        {
            var cardinality = context.slotinformation()?.cardinality();
            var slotname = context.slotinformation()?.slotname();

            return new EtlTemplateInformationSlot(
                cardinality switch { null => null, var x => ConvertCardinality(x) },
                slotname switch { null => null, var x => ConvertSlotname(x) });
        }

        private string ConvertSlotexpressionconstraint(SlotexpressionconstraintContext context)
        {
            return context.GetText().Trim();
        }

        private Cardinality ConvertCardinality(CardinalityContext context)
        {
            return new Cardinality(
                ConvertNonnegativeintegervalue(context.minvalue().nonnegativeintegervalue()),
                context.maxvalue().nonnegativeintegervalue() switch { null => null, var x => ConvertNonnegativeintegervalue(x) });
        }

        private int ConvertNonnegativeintegervalue(NonnegativeintegervalueContext context)
        {
            return int.Parse(context.GetText());
        }
    }
}

