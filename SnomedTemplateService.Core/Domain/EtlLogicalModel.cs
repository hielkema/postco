using SnomedTemplateService.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SnomedTemplateService.Core.Domain
{
    public enum DefinitionStatusEnum
    {
        equivalentTo,
        subtypeOf
    }


    public interface IEtlExpressionNode
    {
        bool ContainsSlot();
    }
    public class EtlExpressionTemplate : IEtlExpressionNode
    {
        public EtlExpressionTemplate(OneOf<DefinitionStatusEnum, EtlTokenReplacementSlot> defintionStatus, EtlSubExpression subExpression)
        {
            DefinitionStatus = defintionStatus ?? throw new ArgumentNullException(nameof(defintionStatus));
            SubExpression = subExpression ?? throw new ArgumentNullException(nameof(subExpression));
        }

        public OneOf<DefinitionStatusEnum, EtlTokenReplacementSlot> DefinitionStatus { get; }
        public EtlSubExpression SubExpression { get; }

        public bool ContainsSlot()
        {
            return DefinitionStatus.Handle(defEnum => false, slot => true) || SubExpression.ContainsSlot();
        }
    }

    public class EtlSubExpression : IEtlExpressionNode
    {
        public IList<(EtlTemplateInformationSlot info, EtlFocusConcept focus)> FocusConcepts { get; }
        public OneOf<EtlAttributeSetRefinement, EtlAttributeGroupRefinement> Refinement { get; }

        public EtlSubExpression(IList<(EtlTemplateInformationSlot info, EtlFocusConcept focus)> focusConcepts, OneOf<EtlAttributeSetRefinement, EtlAttributeGroupRefinement> refinement)
        {
            if (focusConcepts is null)
            {
                throw new ArgumentNullException(nameof(focusConcepts));
            }
            if (focusConcepts.Count == 0)
            {
                throw new ArgumentException($"{nameof(focusConcepts)} should be non-empty", nameof(focusConcepts));
            }
            FocusConcepts = focusConcepts.Select(fc =>
                (fc.focus.ContainsSlot() ? fc.info ?? new EtlTemplateInformationSlot()
                                         : new EtlEmptyInformationSlot(), 
                                            fc.focus)
                ).ToList();
            Refinement = refinement;
        }

        public bool ContainsSlot()
        {
            return FocusConcepts.Any(fc => fc.focus.ContainsSlot()) || Refinement.Handle(setRefinement => setRefinement.ContainsSlot(), groupRefinement => groupRefinement.ContainsSlot());
        }
    }


    public class EtlFocusConcept : EtlConceptReference
    {
        public EtlFocusConcept(OneOf<ConceptReference, EtlConceptReplacementSlot, EtlExpressionReplacementSlot> conceptReference) : base(conceptReference)
        {
        }

        public EtlFocusConcept(EtlConceptReference conceptReference) : base(conceptReference)
        {
        }
    }

    public class ConceptReference
    {
        public ConceptReference(ulong sctId, string term)
        {
            SctId = sctId;
            Term = term;
        }

        public ulong SctId { get; }
        public string Term { get; }
    }

    public class EtlConceptReference : IEtlExpressionNode
    {
        public EtlConceptReference(OneOf<ConceptReference, EtlConceptReplacementSlot, EtlExpressionReplacementSlot> conceptReference)
        {
            ConceptReference = conceptReference ?? throw new ArgumentNullException(nameof(conceptReference));
        }

        public EtlConceptReference(EtlConceptReference other) : this(other.ConceptReference)
        {
        }

        public OneOf<ConceptReference, EtlConceptReplacementSlot, EtlExpressionReplacementSlot> ConceptReference { get; }

        public bool ContainsSlot()
        {
            return ConceptReference.Handle(conceptRef => false, conceptSlot => true, exprSlot => true);
        }
    }

    public class EtlAttributeSetRefinement : IEtlExpressionNode
    {
        public EtlAttributeSetRefinement(EtlAttributeSet attributeSet, IList<(EtlTemplateInformationSlot info, EtlAttributeGroup group)> attributeGroups)
        {
            AttributeSet = attributeSet ?? throw new ArgumentNullException(nameof(attributeSet));
            AttributeGroups = attributeGroups?.Select(ag =>
                ( info: ag.group.ContainsSlot() 
                    ? ag.info ?? new EtlTemplateInformationSlot()
                    : new EtlEmptyInformationSlot(), ag.group
                )
            )?.ToList();
        }

        public EtlAttributeSet AttributeSet { get; }
        public IList<(EtlTemplateInformationSlot info, EtlAttributeGroup group)> AttributeGroups { get; }

        public bool ContainsSlot()
        {
            return AttributeSet.ContainsSlot() || AttributeGroups.Any(group => group.group.ContainsSlot());
        }
    }

    public class EtlAttributeGroupRefinement : IEtlExpressionNode
    {
        public EtlAttributeGroupRefinement(IList<(EtlTemplateInformationSlot info, EtlAttributeGroup group)> attributeGroups)
        {
            if (attributeGroups.Count == 0)
            {
                throw new ArgumentException($"{nameof(attributeGroups)} should be non-empty", nameof(attributeGroups));
            }
            AttributeGroups = attributeGroups.Select(ag => 
                (info: ag.group.ContainsSlot() ?
                    ag.info ?? new EtlTemplateInformationSlot()
                    : new EtlEmptyInformationSlot(), 
                ag.group)
            ).ToList();
        }

        public IList<(EtlTemplateInformationSlot info, EtlAttributeGroup group)> AttributeGroups { get; }

        public bool ContainsSlot()
        {
            return AttributeGroups.Any(group => group.group.ContainsSlot());
        }
    }

    public class EtlAttributeGroup : IEtlExpressionNode
    {
        public EtlAttributeGroup(EtlAttributeSet attributeSet)
        {
            AttributeSet = attributeSet ?? throw new ArgumentNullException(nameof(attributeSet));
        }

        public EtlAttributeSet AttributeSet { get; }

        public bool ContainsSlot()
        {
            return AttributeSet.ContainsSlot(); 
        }
    }

    public class EtlAttributeSet : IEtlExpressionNode
    {
        public EtlAttributeSet(IList<(EtlTemplateInformationSlot info, EtlAttribute attr)> attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }
            if (attributes.Count == 0)
            {
                throw new ArgumentException($"{nameof(attributes)} should be non-empty", nameof(attributes));
            }
            Attributes = attributes.Select(
                ia => (
                    info: ia.attr.ContainsSlot() ?
                        ia.info ?? new EtlTemplateInformationSlot()
                        : new EtlEmptyInformationSlot(),
                    ia.attr)).ToList();
        }

        public IList<(EtlTemplateInformationSlot info, EtlAttribute attr)> Attributes { get; }

        public bool ContainsSlot()
        {
            return Attributes.Any(attr => attr.attr.ContainsSlot());
        }
    }

    public class EtlAttribute : IEtlExpressionNode
    {
        public EtlAttribute(
            EtlAttributeName attributeName,
            OneOf<EtlExpressionValue, EtlConcreteValue> attributeValue
            )
        {
            AttributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            AttributeValue = attributeValue ?? throw new ArgumentNullException(nameof(attributeValue));
        }

        public EtlAttributeName AttributeName { get; }
        public OneOf<EtlExpressionValue, EtlConcreteValue> AttributeValue { get; }

        public bool ContainsSlot()
        {
            return AttributeName.ContainsSlot() || AttributeValue.Handle(expr => expr.ContainsSlot(), concrete => concrete.ContainsSlot());
        }
    }

    public class EtlAttributeName : EtlConceptReference, IEtlExpressionNode
    {
        public EtlAttributeName(OneOf<ConceptReference, EtlConceptReplacementSlot, EtlExpressionReplacementSlot> conceptReference) : base(conceptReference)
        {
        }

        public EtlAttributeName(EtlConceptReference conceptReference) : base(conceptReference)
        {
        }
    }

    public class EtlExpressionValue : IEtlExpressionNode
    {
        public EtlExpressionValue(OneOf<EtlSubExpression, EtlConceptReference> value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public OneOf<EtlSubExpression, EtlConceptReference> Value { get; }

        public bool ContainsSlot()
        {
            return Value.Handle(subExp => subExp.ContainsSlot(), conceptRef => conceptRef.ContainsSlot());
        }
    }

    public class EtlConcreteValue : IEtlExpressionNode
    {
        public EtlConcreteValue(OneOf<int, EtlIntReplacementSlot, decimal, EtlDecimalReplacementSlot, string, EtlStringReplacementSlot, bool, EtlBooleanReplacementSlot> value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public OneOf<
                int, EtlIntReplacementSlot,
                decimal, EtlDecimalReplacementSlot,
                string, EtlStringReplacementSlot,
                bool, EtlBooleanReplacementSlot> Value
        { get; }

        public bool ContainsSlot()
        {
            return Value.Handle(
                intLit => false,
                intSlot => true,
                decimalLit => false,
                decimalSlot => true,
                stringLit => false,
                stringSlot => true,
                boolLit => false,
                boolSlot => true
                );
            ;
        }
    }

    public abstract class EtlTemplateSlot : IEtlExpressionNode
    {
        protected EtlTemplateSlot(string slotName)
        {
            SlotName = slotName;
        }

        public string SlotName { get; }

        public virtual bool ContainsSlot()
        {
            return true;
        }
    }

    public class Cardinality
    {
        public Cardinality()
        {
            MinCardinality = 1;
            MaxCardinality = null;
        }

        public Cardinality(int minCardinality, int? maxCardinality)
        {
            MinCardinality = minCardinality;
            MaxCardinality = maxCardinality;
        }

        public int MinCardinality { get; }
        public int? MaxCardinality { get; }
    }


    public class EtlTemplateInformationSlot : EtlTemplateSlot
    {
        public EtlTemplateInformationSlot() : this(null, null)
        {            
        }
        public EtlTemplateInformationSlot(Cardinality cardinality, string slotName) : base(slotName)
        {
            Cardinality = cardinality ?? new Cardinality();
        }

        public virtual Cardinality Cardinality { get; }

        public virtual bool IsEmpty()
        {
            return false;
        }

        public override bool ContainsSlot()
        {
            return false;
        }
    }

    public class EtlEmptyInformationSlot : EtlTemplateInformationSlot
    {
        public EtlEmptyInformationSlot()
        {
        }

        public override bool IsEmpty()
        {
            return true;   
        }

        public override Cardinality Cardinality => new Cardinality(1, 1);
    }

    public class EtlTokenReplacementSlot : EtlTemplateSlot
    {
        public EtlTokenReplacementSlot(ICollection<SlotToken> slotTokens, string slotName) : base(slotName)
        {
            SlotTokens = slotTokens;
        }

        public ICollection<SlotToken> SlotTokens { get; }
    }

    public class EtlConceptReplacementSlot : EtlTemplateSlot
    {
        public EtlConceptReplacementSlot(string expressionConstraint, string slotName) : base(slotName)
        {
            ExpressionConstraint = expressionConstraint ?? "";
        }

        public string ExpressionConstraint { get; }
    }

    public class EtlExpressionReplacementSlot : EtlTemplateSlot
    {
        public EtlExpressionReplacementSlot(string expressionConstraint, string slotName) : base(slotName)
        {
            ExpressionConstraint = expressionConstraint ?? "";
        }
        public string ExpressionConstraint { get; }
    }

    public class EtlStringReplacementSlot : EtlTemplateSlot
    {
        public ICollection<string> Values { get; }

        public EtlStringReplacementSlot(ICollection<string> values, string slotName) : base(slotName)
        {
            Values = values.Where(v => v != null).ToList();
        }
    }

    public class EtlIntReplacementSlot : EtlTemplateSlot
    {
        public EtlIntReplacementSlot(string slotName) : base(slotName)
        {
            Values = new List<int>();
            Ranges = new List<Range>();
        }

        public ICollection<int> Values { get; }

        public ICollection<Range> Ranges { get; }

        public void AddRange(bool? exclusiveMinimum = null, int? minimumValue = null, bool? exclusiveMaximum = null, int? maximumValue = null)
        {
            Ranges.Add(new Range(exclusiveMinimum, minimumValue, exclusiveMaximum, maximumValue));
        }
        public class Range
        {
            public Range(bool? exclusiveMinimum, int? minimumValue, bool? exclusiveMaximum, int? maximumValue)
            {
                ExclusiveMinimum = exclusiveMinimum;
                MinimumValue = minimumValue;
                ExclusiveMaximum = exclusiveMaximum;
                MaximumValue = maximumValue;
            }

            public bool? ExclusiveMinimum { get; }
            public int? MinimumValue { get; }
            public bool? ExclusiveMaximum { get; }
            public int? MaximumValue { get; }
        }
    }
    public class EtlDecimalReplacementSlot : EtlTemplateSlot
    {
        public EtlDecimalReplacementSlot(string slotName) : base(slotName)
        {
            Ranges = new List<Range>();
            Values = new List<decimal>();
        }
        public ICollection<Range> Ranges { get; }

        public ICollection<decimal> Values { get; }
        public void AddRange(bool? exclusiveMinimum = null, decimal? minimumValue = null, bool? exclusiveMaximum = null, decimal? maximumValue = null)
        {
            Ranges.Add(new Range(exclusiveMinimum, minimumValue, exclusiveMaximum, maximumValue));
        }
        public class Range
        {
            public Range(bool? exclusiveMinimum, decimal? minimumValue, bool? exclusiveMaximum, decimal? maximumValue)
            {
                ExclusiveMinimum = exclusiveMinimum;
                MinimumValue = minimumValue;
                ExclusiveMaximum = exclusiveMaximum;
                MaximumValue = maximumValue;
            }

            public bool? ExclusiveMinimum { get; }
            public decimal? MinimumValue { get; }
            public bool? ExclusiveMaximum { get; }
            public decimal? MaximumValue { get; }
        }
    }

    public class EtlBooleanReplacementSlot : EtlTemplateSlot
    {
        public ICollection<bool> Values { get; }

        public EtlBooleanReplacementSlot(ICollection<bool> values, string slotName) : base(slotName)
        {
            Values = values;
        }
    }

    public enum SlotToken
    {
        definitionstatus,
        memberof,
        constraintoperator,
        conjunction,
        disjunction,
        exclusion,
        reverseflag,
        expressioncomparisonoperator,
        numericcomparisonoperator,
        stringcomparisonoperator,
        booleancomparisonoperator
    }
}
