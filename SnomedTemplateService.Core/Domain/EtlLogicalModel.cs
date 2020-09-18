using SnomedTemplateService.Util;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SnomedTemplateService.Core.Domain
{
    public enum DefinitionStatusEnum
    {
        equivalentTo,
        subtypeOf
    }

    public class EtlExpressionTemplate
    {
        public SwitchType<DefinitionStatusEnum, EtlTokenReplacementSlot> DefintionStatus { get; }
        public EtlSubExpression SubExpression { get; }
    }

    public class EtlSubExpression
    {
        public ICollection<(EtlTemplateInformationSlot, EtlFocusConcept)> FocusConcepts;
        public EtlRefinement Refinement;  
    } 


    public class EtlFocusConcept : EtlConceptReference
    {
    }

    public class ConceptReference
    {
        public int SctId { get;  }
        public string Term { get; }
    }

    public class EtlConceptReference
    {
        public SwitchType<ConceptReference, EtlConceptReplacementSlot, EtlExpressionReplacementSlot> ConceptReference { get; }
    }

    public class EtlRefinement
    {
        
    }

    public class EtlAttributeSetRefinement : EtlRefinement
    {
        public EtlAttributeSet AttributeSet { get; }
        public ICollection<(EtlTemplateInformationSlot, EtlAttributeGroup)> AttributeGroups { get; } 
    }

    public class EtlAttributeGroupRefinement : EtlRefinement
    {
        public ICollection<(EtlTemplateInformationSlot, EtlAttributeGroup)> AttributeGroups { get; }
    }

    public class EtlAttributeGroup
    {
        public EtlAttributeSet AttributeSet { get; }
    }

    public class EtlAttributeSet
    {
        ICollection<(EtlTemplateInformationSlot, EtlAttribute)> Attributes { get; }
    }

    public class EtlAttribute
    { 
        public EtlAttributeName AttributeName { get; }
        public EtlAttributeValue AttributeValue { get; }
    }

    public class EtlAttributeName : EtlConceptReference
    {
    }

    public class EtlAttributeValue
    {
    }

    public class EtlExpressionValue : EtlAttributeValue
    {
        public SwitchType<EtlSubExpression, EtlConceptReference> Value { get; }
    }

    public class EtlConcreteValue : EtlAttributeValue 
    {
        public SwitchType<
                int, EtlIntValueReplacementSlot,
                decimal, EtlDecimalValueReplacementSlot,
                string, EtlStringValueReplacementSlot,
                bool, EtlBooleanValueReplacementSlot> Value
        { get; }
    }

    public abstract class EtlTemplateSlot
    {
        public string SlotName { get; }
    }

    public class Cardinality
    {
        public int MinCardinality { get; }
        public int? MaxCardinality { get; }
    }
    public class EtlTemplateInformationSlot : EtlTemplateSlot 
    {
        public Cardinality Cardinality { get; }
    }

    public class EtlTemplateReplacementSlot : EtlTemplateSlot
    {

    }

    public class EtlTokenReplacementSlot : EtlTemplateReplacementSlot
    {
        public ICollection<SlotToken> SlotTokens { get; }
    }

    public class EtlConceptReplacementSlot : EtlTemplateReplacementSlot
    {
        public string ExpressionConstraint { get; }
    }

    public class EtlExpressionReplacementSlot : EtlTemplateReplacementSlot
    {
        public string ExpressionConstraint { get; }
    }

    public class EtlConcreteValueReplacementSlot
    {
    }

    public class EtlStringValueReplacementSlot : EtlConcreteValueReplacementSlot
    {
        public ICollection<string> Values;
    }

    public class EtlIntValueReplacementSlot : EtlConcreteValueReplacementSlot
    {
        public ICollection<Range> Values { get; }
        public class Range
        {
            public bool ExclusiveMinimum { get; }
            public int MinimumValue { get; }
            public bool ExclusiveMaximum { get; }
            public int MaximumValue { get; }
        }
    }
    public class EtlDecimalValueReplacementSlot : EtlConcreteValueReplacementSlot
    {
        public ICollection<Range> Values { get; }
        public class Range
        {
            public bool ExclusiveMinimum { get; }
            public decimal MinimumValue { get; }
            public bool ExclusiveMaximum { get; }
            public decimal MaximumValue { get; }
        }
    }

    public class EtlBooleanValueReplacementSlot : EtlConcreteValueReplacementSlot
    {
        public ICollection<bool> Values; 
    }

    public enum SlotToken
    {
        definitionStatus,
        memberOf,
        constraintOperator,
        conjunction,
        disjunction,
        exclusion,
        reverseFlag,
        expressionComparisonOperator,
        numericComparisonOperator,
        stringComparisonOperator
    }
}