using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using SnomedTemplateService.Core.Domain;
using SnomedTemplateService.Core.Interfaces;
using SnomedTemplateService.Parser.Generated;

namespace SnomedTemplateService.Parser
{
    public class EtlParserService : IEtlParserService
    {
        public EtlExpressionTemplate ParseEtlString(string template)
        {
            throw new NotImplementedException();
        }
    }

    public class ExpressionTemplateListener : ExpressionTemplateBaseListener
    {
        public EtlExpressionTemplate expressionTemplate;
        public override void EnterAnynonescapedchar([NotNull] ExpressionTemplateParser.AnynonescapedcharContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterAttribute([NotNull] ExpressionTemplateParser.AttributeContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterAttributegroup([NotNull] ExpressionTemplateParser.AttributegroupContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterAttributename([NotNull] ExpressionTemplateParser.AttributenameContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterAttributeset([NotNull] ExpressionTemplateParser.AttributesetContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterAttributevalue([NotNull] ExpressionTemplateParser.AttributevalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterBooleanreplacement([NotNull] ExpressionTemplateParser.BooleanreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterBooleanvalue([NotNull] ExpressionTemplateParser.BooleanvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterBs([NotNull] ExpressionTemplateParser.BsContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterComment([NotNull] ExpressionTemplateParser.CommentContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterConceptid([NotNull] ExpressionTemplateParser.ConceptidContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterConceptreference([NotNull] ExpressionTemplateParser.ConceptreferenceContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterConceptreplacement([NotNull] ExpressionTemplateParser.ConceptreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterConceptreplacementslot([NotNull] ExpressionTemplateParser.ConceptreplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterConcretevaluereplacement([NotNull] ExpressionTemplateParser.ConcretevaluereplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterConcretevaluereplacementslot([NotNull] ExpressionTemplateParser.ConcretevaluereplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterCr([NotNull] ExpressionTemplateParser.CrContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterDecimalreplacement([NotNull] ExpressionTemplateParser.DecimalreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterDecimalvalue([NotNull] ExpressionTemplateParser.DecimalvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterDefinitionstatus([NotNull] ExpressionTemplateParser.DefinitionstatusContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterDigit([NotNull] ExpressionTemplateParser.DigitContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterDigitnonzero([NotNull] ExpressionTemplateParser.DigitnonzeroContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterEquivalentto([NotNull] ExpressionTemplateParser.EquivalenttoContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterEscapedchar([NotNull] ExpressionTemplateParser.EscapedcharContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterEveryRule([NotNull] ParserRuleContext ctx)
        {
            throw new NotImplementedException();
        }


        public override void EnterExclusivemaximum([NotNull] ExpressionTemplateParser.ExclusivemaximumContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterExclusiveminimum([NotNull] ExpressionTemplateParser.ExclusiveminimumContext context)
        {
            throw new NotImplementedException();
        }


        public override void EnterExpressionreplacement([NotNull] ExpressionTemplateParser.ExpressionreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterExpressionreplacementslot([NotNull] ExpressionTemplateParser.ExpressionreplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterExpressiontemplate([NotNull] ExpressionTemplateParser.ExpressiontemplateContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterExpressionvalue([NotNull] ExpressionTemplateParser.ExpressionvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterFalse_1([NotNull] ExpressionTemplateParser.False_1Context context)
        {
            throw new NotImplementedException();
        }

        public override void EnterFocusconcept([NotNull] ExpressionTemplateParser.FocusconceptContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterHtab([NotNull] ExpressionTemplateParser.HtabContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterIntegerreplacement([NotNull] ExpressionTemplateParser.IntegerreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterIntegervalue([NotNull] ExpressionTemplateParser.IntegervalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterLf([NotNull] ExpressionTemplateParser.LfContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterNonquotestringvalue([NotNull] ExpressionTemplateParser.NonquotestringvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterNonwsnonpipe([NotNull] ExpressionTemplateParser.NonwsnonpipeContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterNumericvalue([NotNull] ExpressionTemplateParser.NumericvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterParse([NotNull] ExpressionTemplateParser.ParseContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterQm([NotNull] ExpressionTemplateParser.QmContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterRefinement([NotNull] ExpressionTemplateParser.RefinementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSctid([NotNull] ExpressionTemplateParser.SctidContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotbooleanset([NotNull] ExpressionTemplateParser.SlotbooleansetContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotbooleanvalue([NotNull] ExpressionTemplateParser.SlotbooleanvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotdecimalmaximum([NotNull] ExpressionTemplateParser.SlotdecimalmaximumContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotdecimalminimum([NotNull] ExpressionTemplateParser.SlotdecimalminimumContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotdecimalrange([NotNull] ExpressionTemplateParser.SlotdecimalrangeContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotdecimalset([NotNull] ExpressionTemplateParser.SlotdecimalsetContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotdecimalvalue([NotNull] ExpressionTemplateParser.SlotdecimalvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotinformation([NotNull] ExpressionTemplateParser.SlotinformationContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotintegermaximum([NotNull] ExpressionTemplateParser.SlotintegermaximumContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotintegerminimum([NotNull] ExpressionTemplateParser.SlotintegerminimumContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotintegerrange([NotNull] ExpressionTemplateParser.SlotintegerrangeContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotintegerset([NotNull] ExpressionTemplateParser.SlotintegersetContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotintegervalue([NotNull] ExpressionTemplateParser.SlotintegervalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotname([NotNull] ExpressionTemplateParser.SlotnameContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotstring([NotNull] ExpressionTemplateParser.SlotstringContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlotstringset([NotNull] ExpressionTemplateParser.SlotstringsetContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlottoken([NotNull] ExpressionTemplateParser.SlottokenContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSlottokenset([NotNull] ExpressionTemplateParser.SlottokensetContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSp([NotNull] ExpressionTemplateParser.SpContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterStringreplacement([NotNull] ExpressionTemplateParser.StringreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterStringvalue([NotNull] ExpressionTemplateParser.StringvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSubexpression([NotNull] ExpressionTemplateParser.SubexpressionContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterSubtypeof([NotNull] ExpressionTemplateParser.SubtypeofContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterTemplateinformationslot([NotNull] ExpressionTemplateParser.TemplateinformationslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterTemplatereplacementslot([NotNull] ExpressionTemplateParser.TemplatereplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterTemplateslot([NotNull] ExpressionTemplateParser.TemplateslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterTerm([NotNull] ExpressionTemplateParser.TermContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterTokenreplacement([NotNull] ExpressionTemplateParser.TokenreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterTokenreplacementslot([NotNull] ExpressionTemplateParser.TokenreplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterTrue_1([NotNull] ExpressionTemplateParser.True_1Context context)
        {
            throw new NotImplementedException();
        }

        public override void EnterUtf8_2([NotNull] ExpressionTemplateParser.Utf8_2Context context)
        {
            throw new NotImplementedException();
        }

        public override void EnterUtf8_3([NotNull] ExpressionTemplateParser.Utf8_3Context context)
        {
            throw new NotImplementedException();
        }

        public override void EnterUtf8_4([NotNull] ExpressionTemplateParser.Utf8_4Context context)
        {
            throw new NotImplementedException();
        }

        public override void EnterUtf8_tail([NotNull] ExpressionTemplateParser.Utf8_tailContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterWs([NotNull] ExpressionTemplateParser.WsContext context)
        {
            throw new NotImplementedException();
        }

        public override void EnterZero([NotNull] ExpressionTemplateParser.ZeroContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitAnynonescapedchar([NotNull] ExpressionTemplateParser.AnynonescapedcharContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitAttribute([NotNull] ExpressionTemplateParser.AttributeContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitAttributegroup([NotNull] ExpressionTemplateParser.AttributegroupContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitAttributename([NotNull] ExpressionTemplateParser.AttributenameContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitAttributeset([NotNull] ExpressionTemplateParser.AttributesetContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitAttributevalue([NotNull] ExpressionTemplateParser.AttributevalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitBooleanreplacement([NotNull] ExpressionTemplateParser.BooleanreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitBooleanvalue([NotNull] ExpressionTemplateParser.BooleanvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitBs([NotNull] ExpressionTemplateParser.BsContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitConceptid([NotNull] ExpressionTemplateParser.ConceptidContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitConceptreference([NotNull] ExpressionTemplateParser.ConceptreferenceContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitConceptreplacement([NotNull] ExpressionTemplateParser.ConceptreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitConceptreplacementslot([NotNull] ExpressionTemplateParser.ConceptreplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitConcretevaluereplacement([NotNull] ExpressionTemplateParser.ConcretevaluereplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitConcretevaluereplacementslot([NotNull] ExpressionTemplateParser.ConcretevaluereplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitCr([NotNull] ExpressionTemplateParser.CrContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitDecimalreplacement([NotNull] ExpressionTemplateParser.DecimalreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitDecimalvalue([NotNull] ExpressionTemplateParser.DecimalvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitDefinitionstatus([NotNull] ExpressionTemplateParser.DefinitionstatusContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitDigit([NotNull] ExpressionTemplateParser.DigitContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitDigitnonzero([NotNull] ExpressionTemplateParser.DigitnonzeroContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitEquivalentto([NotNull] ExpressionTemplateParser.EquivalenttoContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitEscapedchar([NotNull] ExpressionTemplateParser.EscapedcharContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitEveryRule([NotNull] ParserRuleContext ctx)
        {
            throw new NotImplementedException();
        }

        public override void ExitExclusivemaximum([NotNull] ExpressionTemplateParser.ExclusivemaximumContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitExclusiveminimum([NotNull] ExpressionTemplateParser.ExclusiveminimumContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitExpressionreplacement([NotNull] ExpressionTemplateParser.ExpressionreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitExpressionreplacementslot([NotNull] ExpressionTemplateParser.ExpressionreplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitExpressiontemplate([NotNull] ExpressionTemplateParser.ExpressiontemplateContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitExpressionvalue([NotNull] ExpressionTemplateParser.ExpressionvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitFalse_1([NotNull] ExpressionTemplateParser.False_1Context context)
        {
            throw new NotImplementedException();
        }

        public override void ExitFocusconcept([NotNull] ExpressionTemplateParser.FocusconceptContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitHtab([NotNull] ExpressionTemplateParser.HtabContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitIntegerreplacement([NotNull] ExpressionTemplateParser.IntegerreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitIntegervalue([NotNull] ExpressionTemplateParser.IntegervalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitLf([NotNull] ExpressionTemplateParser.LfContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitNonquotestringvalue([NotNull] ExpressionTemplateParser.NonquotestringvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitNonwsnonpipe([NotNull] ExpressionTemplateParser.NonwsnonpipeContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitNumericvalue([NotNull] ExpressionTemplateParser.NumericvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitParse([NotNull] ExpressionTemplateParser.ParseContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitQm([NotNull] ExpressionTemplateParser.QmContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitRefinement([NotNull] ExpressionTemplateParser.RefinementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSctid([NotNull] ExpressionTemplateParser.SctidContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotbooleanset([NotNull] ExpressionTemplateParser.SlotbooleansetContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotbooleanvalue([NotNull] ExpressionTemplateParser.SlotbooleanvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotdecimalmaximum([NotNull] ExpressionTemplateParser.SlotdecimalmaximumContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotdecimalminimum([NotNull] ExpressionTemplateParser.SlotdecimalminimumContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotdecimalrange([NotNull] ExpressionTemplateParser.SlotdecimalrangeContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotdecimalset([NotNull] ExpressionTemplateParser.SlotdecimalsetContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotdecimalvalue([NotNull] ExpressionTemplateParser.SlotdecimalvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotinformation([NotNull] ExpressionTemplateParser.SlotinformationContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotintegermaximum([NotNull] ExpressionTemplateParser.SlotintegermaximumContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotintegerminimum([NotNull] ExpressionTemplateParser.SlotintegerminimumContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotintegerrange([NotNull] ExpressionTemplateParser.SlotintegerrangeContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotintegerset([NotNull] ExpressionTemplateParser.SlotintegersetContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotintegervalue([NotNull] ExpressionTemplateParser.SlotintegervalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotname([NotNull] ExpressionTemplateParser.SlotnameContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotstring([NotNull] ExpressionTemplateParser.SlotstringContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlotstringset([NotNull] ExpressionTemplateParser.SlotstringsetContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlottoken([NotNull] ExpressionTemplateParser.SlottokenContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSlottokenset([NotNull] ExpressionTemplateParser.SlottokensetContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSp([NotNull] ExpressionTemplateParser.SpContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitStringreplacement([NotNull] ExpressionTemplateParser.StringreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitStringvalue([NotNull] ExpressionTemplateParser.StringvalueContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSubexpression([NotNull] ExpressionTemplateParser.SubexpressionContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitSubtypeof([NotNull] ExpressionTemplateParser.SubtypeofContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitTemplateinformationslot([NotNull] ExpressionTemplateParser.TemplateinformationslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitTemplatereplacementslot([NotNull] ExpressionTemplateParser.TemplatereplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitTemplateslot([NotNull] ExpressionTemplateParser.TemplateslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitTerm([NotNull] ExpressionTemplateParser.TermContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitTokenreplacement([NotNull] ExpressionTemplateParser.TokenreplacementContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitTokenreplacementslot([NotNull] ExpressionTemplateParser.TokenreplacementslotContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitTrue_1([NotNull] ExpressionTemplateParser.True_1Context context)
        {
            throw new NotImplementedException();
        }

        public override void ExitUtf8_2([NotNull] ExpressionTemplateParser.Utf8_2Context context)
        {
            throw new NotImplementedException();
        }

        public override void ExitUtf8_3([NotNull] ExpressionTemplateParser.Utf8_3Context context)
        {
            throw new NotImplementedException();
        }

        public override void ExitUtf8_4([NotNull] ExpressionTemplateParser.Utf8_4Context context)
        {
            throw new NotImplementedException();
        }

        public override void ExitUtf8_tail([NotNull] ExpressionTemplateParser.Utf8_tailContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitWs([NotNull] ExpressionTemplateParser.WsContext context)
        {
            throw new NotImplementedException();
        }

        public override void ExitZero([NotNull] ExpressionTemplateParser.ZeroContext context)
        {
            throw new NotImplementedException();
        }

        public override void VisitErrorNode([NotNull] IErrorNode node)
        {
            throw new NotImplementedException();
        }

        public override void VisitTerminal([NotNull] ITerminalNode node)
        {
            throw new NotImplementedException();
        }
    }
}
