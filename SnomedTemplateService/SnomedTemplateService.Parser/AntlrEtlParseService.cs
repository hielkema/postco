using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SnomedTemplateService.Core.Domain.Etl;
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
        public ExpressionTemplate ParseExpressionTemplate(string s)
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

        private ExpressionTemplate ConvertExpressiontemplate(ExpressiontemplateContext context)
        {
            var constDefStatus = context.definitionstatus();
            var tokenSlot = context.tokenreplacementslot();
            var subexpression = context.subexpression();

           IDefinitionStatusOrSlot defStatus;
            if ((new object[] { constDefStatus, tokenSlot }).Count(s => s != null) > 1)
            {
                throw new ParserException();
            }

            if (constDefStatus != null)
            {
                defStatus = ConvertDefinitionstatus(constDefStatus);
            }
            else if (tokenSlot != null)
            {
                defStatus = ConvertTokenreplacementslot(tokenSlot);
            }
            else
            {
                defStatus = new DefinitionStatusLiteral(DefinitionStatusEnum.equivalentTo);
            }
            return subexpression switch 
            {
                null => throw new ParserException(), 
                var su => new ExpressionTemplate(defStatus, ConvertSubexpression(su)) 
            };
        }

        private Subexpression ConvertSubexpression(SubexpressionContext context)
        {
            return new Subexpression(
                ConvertFocusconcept(context.focusconcept()),
                context.refinement() switch
                {
                    null => null,
                    var r => ConvertRefinement(r)
                });
        }

        private DefinitionStatusLiteral ConvertDefinitionstatus(DefinitionstatusContext context)
        {
            switch (context.GetText())
            {
                case "===":
                    return new DefinitionStatusLiteral(DefinitionStatusEnum.equivalentTo);

                case "<<<":
                    return new DefinitionStatusLiteral(DefinitionStatusEnum.subtypeOf);

                default:
                    throw new ParserException();
            }
        }

        private IList<(TemplateInformationSlot info, IConceptReferenceOrSlot focusConcept)> ConvertFocusconcept(FocusconceptContext context)
        {
            var result = new List<(TemplateInformationSlot info, IConceptReferenceOrSlot focusConcept)>();
            TemplateInformationSlot currentInformationSlot = null;
            foreach (var child in context.children)
            {
                switch (child)
                {
                    case null:
                        throw new ParserException();
                    case TemplateinformationslotContext tic:
                        var ti = ConvertTemplateinformationslot(tic);
                        currentInformationSlot = ti;
                        break;
                    case ConceptreferenceContext cr:
                        result.Add((currentInformationSlot, ConvertConceptreference(cr)));
                        currentInformationSlot = null;
                        break;
                    default:
                        if (!Regex.IsMatch(child.GetText(), @"^(?:[\u0020\t\r\n]+|\+)$"))
                        {
                            throw new Exception("syntax error");
                        }
                        break;
                }
            }
            if (currentInformationSlot != null)
            {
                throw new ParserException();
            }
            return result;
        }

        private IConceptReferenceOrSlot ConvertConceptreference(ConceptreferenceContext context)
        {
            var conceptSlotContext = context.conceptreplacementslot();
            var expressionSlotContext = context.expressionreplacementslot();
            var conceptIdContext = context.conceptid();
            var termContext = context.term();

            if ((new object[] { conceptSlotContext, expressionSlotContext, conceptIdContext }).Count(c => c != null) != 1)
            {
                throw new ParserException();
            }
            if (conceptSlotContext != null)
            {
                return ConvertConceptreplacementslot(conceptSlotContext);
            }
            if (expressionSlotContext != null)
            {
                return ConvertExpressionreplacementslot(expressionSlotContext);
            }
            if (conceptIdContext != null)
            {
                ulong conceptId = ConvertConceptid(conceptIdContext);
                string term = null;

                if (termContext != null)
                {
                    term = ConvertTerm(termContext);
                }
                return new ConceptReference(conceptId, term);
            }
            throw new ParserException();
        }

        private ulong ConvertConceptid(ConceptidContext context)
        {
            return ulong.Parse(context.GetText());
        }

        private string ConvertTerm(TermContext context)
        {
            return context.GetText();
        }

        private IAttributeSetOrGroupRefinement ConvertRefinement(RefinementContext context)
        {
            var attributesetContext = context.attributeset();

            if (attributesetContext != null)
            {
                return new AttributeSetRefinement(
                    ConvertAttributeset(attributesetContext),
                    context.attributegroup().Select(ag1 => ag1 switch
                    {
                        null => throw new ParserException(),
                        var ag2 => ConvertAttributegroup(ag2)
                    }
                    ).ToList()

                );
            }
            else
            {
                return new AttributeGroupRefinement(
                        context.attributegroup().Select(ag1 => ag1 switch
                        {
                            null => throw new ParserException(),
                            var ag2 => ConvertAttributegroup(ag2)
                        }
                        ).ToList()
                );
            }
        }

        private (TemplateInformationSlot info, AttributeSet group) ConvertAttributegroup(AttributegroupContext context)
        {
            return (context.templateinformationslot() switch { null => null, var ti => ConvertTemplateinformationslot(ti) },
                ConvertAttributeset(context.attributeset()));
        }

        private AttributeSet ConvertAttributeset(AttributesetContext context)
        {
            return new AttributeSet(context.attribute().Select(
                attr => attr switch
                {
                    null => throw new ParserException(),
                    var a => ConvertAttribute(a)
                }
            ).ToList());

        }

        private (TemplateInformationSlot info, EtlAttribute attr) ConvertAttribute(AttributeContext context)
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

        private IConceptReferenceOrSlot ConvertAttributename(AttributenameContext context)
        {
            return ConvertConceptreference(context.conceptreference());
        }

        private IAttributeValueOrSlot ConvertAttributevalue(AttributevalueContext context)
        {
            var expressionvalueContext = context.expressionvalue();
            var stringvalueContext = context.stringvalue();
            var numericvalueContext = context.numericvalue();
            var booleanvalueContext = context.booleanvalue();
            var concretevalueslotContext = context.concretevaluereplacementslot();
            if ((new object[] { expressionvalueContext, stringvalueContext, numericvalueContext,
                booleanvalueContext, concretevalueslotContext}).Count(c => c != null) != 1)
            {
                throw new ParserException();
            }
            if (expressionvalueContext != null)
            {
                return ConvertExpressionvalue(expressionvalueContext).Handle<IAttributeValueOrSlot>(
                    handleSubexpression: e=>e,
                    handleConceptSlot: s=>s,
                    handleExpressionSlot: s=>s
                    );
            }
            else if (numericvalueContext != null)
            {
                return ConvertNumericvalue(numericvalueContext);
            }
            else if (stringvalueContext != null)
            {
                return new StringLiteral(ConvertStringvalue(stringvalueContext));
            }
            else if (booleanvalueContext != null)
            {
                return new BoolLiteral(ConvertBooleanvalue(booleanvalueContext));
            }
            else if (concretevalueslotContext != null)
            {
                return ConvertConcretevaluereplacementslot(concretevalueslotContext);
            }
            else
            {
                throw new ParserException();
            }
        }

        private ISubexpressionOrSlot ConvertExpressionvalue(ExpressionvalueContext context)
        {
            var conceptRef = context.conceptreference();
            var subExp = context.subexpression();
            if ((new object[] { conceptRef, subExp }).Where(x => x != null).Count() != 1)
            {
                throw new ParserException();
            }
            if (conceptRef != null)
            {
                return ConvertConceptreference(conceptRef)
                    .Handle<ISubexpressionOrSlot>(
                        handleConceptReference: c=>new Subexpression(c),
                        handleConceptSlot: s=>s,
                        handleExpressionSlot: s=> s);
            }
            else if (subExp != null)
            {
                return ConvertSubexpression(subExp);
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

        private IAttributeValueOrSlot ConvertNumericvalue(NumericvalueContext context)
        {
            var sign = context.DASH() != null ? -1 : 1; 
            if ((new object[] { context.decimalvalue(), context.integervalue() }).Count(c => c != null) != 1)
            {
                throw new ParserException();
            }
            else if (context.integervalue() != null)
            {
                return new IntLiteral(sign * ConvertIntegervalue(context.integervalue()));
            }
            else if (context.decimalvalue() != null)
            {
                return new DecimalLiteral(sign * ConvertDecimalvalue(context.decimalvalue()));
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

        private ConceptReplacementSlot ConvertConceptreplacementslot(ConceptreplacementslotContext context)
        {
            var slotName = context.slotname() switch { null => null, var s => ConvertSlotname(s) };
            var expressionConstraint = context.conceptreplacement().slotexpressionconstraint() switch { null => null, var e => ConvertSlotexpressionconstraint(e) };
            return new ConceptReplacementSlot(expressionConstraint, slotName);
        }

        private ExpressionReplacementSlot ConvertExpressionreplacementslot(ExpressionreplacementslotContext context)
        {
            var slotName = context.slotname() switch { null => null, var s => ConvertSlotname(s) };
            var expressionConstraint = context.expressionreplacement().slotexpressionconstraint() switch { null => null, var e => ConvertSlotexpressionconstraint(e) };
            return new ExpressionReplacementSlot(expressionConstraint, slotName);
        }

        private TokenReplacementSlot ConvertTokenreplacementslot(TokenreplacementslotContext context)
        {
            var slotName = context.slotname() switch { null => null, var s => ConvertSlotname(s) };
            return new TokenReplacementSlot(ConvertSlottokenset(context.tokenreplacement().slottokenset()), slotName);
        }

        private IAttributeValueOrSlot ConvertConcretevaluereplacementslot(ConcretevaluereplacementslotContext context)
        {
            // TODO: CHECK
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
                return new StringReplacementSlot(
                        stringReplacement.slotstringset() switch
                        {
                            null => throw new ParserException(),
                            var s => ConvertSlotstringset(s)
                        },
                        slotName
                        );
            }
            else if (intReplacement != null)
            {
                var slot = new IntReplacementSlot(slotName);
                var integerReplacements = (intReplacement.slotintegerset() switch
                {
                    null => throw new ParserException(),
                    var i => ConvertSlotintegerset(i)
                }).ToList();

                foreach (var ir in integerReplacements)
                {
                    ir.Handle(i => { slot.Values.Add(i); return ValueTuple.Create(); }, range => { slot.Ranges.Add(range); return ValueTuple.Create(); });
                }

                return slot;
            }
            else if (decimalReplacement != null)
            {
                var slot = new DecimalReplacementSlot(slotName);
                var decimalReplacements = (decimalReplacement.slotdecimalset() switch
                {
                    null => throw new ParserException(),
                    var d => ConvertSlotdecimalset(d)
                }).ToList();

                foreach (var dr in decimalReplacements)
                {
                    dr.Handle(d => { slot.Values.Add(d); return ValueTuple.Create(); }, range => { slot.Ranges.Add(range); return ValueTuple.Create(); });
                }

                return slot;
            }
            else if (boolReplacement != null)
            {
                return new BoolReplacementSlot(
                        boolReplacement.slotbooleanset() switch
                        {
                            null => throw new ParserException(),
                            var b => ConvertSlotbooleanset(b)
                        },
                        slotName);                  
            }
            throw new ParserException();
        }

        private ICollection<DefinitionStatusEnum> ConvertSlottokenset(SlottokensetContext context)
        {
            var tokens = Regex.Split(context.GetText().ToLowerInvariant().Trim(), @"\s+");
            return tokens.SelectMany(
                token => token switch
                {
                    "===" => new[] { DefinitionStatusEnum.equivalentTo },
                    "<<<" => new[] { DefinitionStatusEnum.subtypeOf },
                    _ => Enumerable.Empty<DefinitionStatusEnum>()
                }).ToList();
        }

        private ICollection<string> ConvertSlotstringset(SlotstringsetContext context)
        {
            return context.slotstring().Select(slotstr => ConvertSlotstring(slotstr)).ToList();
        }

        private ICollection<OneOf<int, IntReplacementSlot.Range>> ConvertSlotintegerset(SlotintegersetContext context)
        {
            return context.children.Select<IParseTree, OneOf<int, IntReplacementSlot.Range>>(
                c => c switch
                {
                    SlotintegervalueContext iv => new FirstOf<int, IntReplacementSlot.Range>(ConvertSlotintegervalue(iv)),
                    SlotintegerrangeContext ir => new SecondOf<int, IntReplacementSlot.Range>(ConvertSlotintegerrange(ir)),
                    _ => throw new ParserException()
                }
            ).ToList();
        }

        private ICollection<OneOf<decimal, DecimalReplacementSlot.Range>> ConvertSlotdecimalset(SlotdecimalsetContext context)
        {
            return context.children.Select<IParseTree, OneOf<decimal, DecimalReplacementSlot.Range>>(
                c => c switch
                {
                    SlotdecimalvalueContext dv => new FirstOf<decimal, DecimalReplacementSlot.Range>(ConvertSlotdecimalvalue(dv)),
                    SlotdecimalrangeContext dr => new SecondOf<decimal, DecimalReplacementSlot.Range>(ConvertSlotdecimalrange(dr)),
                    _ => throw new ParserException()
                }
            ).ToList();
        }

        private ICollection<bool> ConvertSlotbooleanset(SlotbooleansetContext context)
        {
            return context.slotbooleanvalue().Select(slotbool => ConvertSlotbooleanvalue(slotbool)).ToList();
        }

        private IntReplacementSlot.Range ConvertSlotintegerrange(SlotintegerrangeContext context)
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

            return new IntReplacementSlot.Range(exclusiveMinimum, minimum, exclusiveMaximum, maximum);
        }

        private int ConvertSlotintegervalue(SlotintegervalueContext context)
        {
            return int.Parse(context.GetText().TrimStart(new[] { '#' }));
        }

        private DecimalReplacementSlot.Range ConvertSlotdecimalrange(SlotdecimalrangeContext context)
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

            return new DecimalReplacementSlot.Range(exclusiveMinimum, minimum, exclusiveMaximum, maximum);
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

        private TemplateInformationSlot ConvertTemplateinformationslot(TemplateinformationslotContext context)
        {
            var cardinality = context.slotinformation()?.cardinality();
            var slotname = context.slotinformation()?.slotname();

            return new TemplateInformationSlot(
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

