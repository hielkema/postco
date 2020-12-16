using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SnomedTemplateService.Core.Domain.Etl
{
    public enum DefinitionStatusEnum
    {
        equivalentTo,
        subtypeOf
    }

    public interface IExpressionNode
    {
        bool ContainsSlot();
    }

    public interface IDefinitionStatusOrSlot : IExpressionNode
    {
        TResult Handle<TResult>(Func<DefinitionStatusEnum, TResult> handleLiteral, Func<TokenReplacementSlot, TResult> handleSlot);
    }

    public interface IAttributeValueOrSlot : IExpressionNode
    {
        TResult Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot,
            Func<IStringOrSlot, TResult> handleStringOrSlot,
            Func<IIntegerOrSlot, TResult> handleIntOrSlot,
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot,
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot
            );
    }
    
    public interface IConceptReferenceOrSlot : IExpressionNode
    {
        TResult Handle<TResult>(
            Func<ConceptReference, TResult> handleConceptReference,
            Func<ConceptReplacementSlot, TResult> handleConceptSlot,
            Func<ExpressionReplacementSlot, TResult> handleExpressionSlot);
    }

    public interface ISubexpressionOrSlot : IExpressionNode
    {
        TResult Handle<TResult>(
            //Func<ConceptReference, TResult> handleConceptReference,
            Func<Subexpression, TResult> handleSubexpression,
            Func<ConceptReplacementSlot, TResult> handleConceptSlot,
            Func<ExpressionReplacementSlot, TResult> handleExpressionSlot);
    }
    
    public interface IStringOrSlot : IExpressionNode
    {
        TResult Handle<TResult>(Func<string, TResult> handleLiteral, Func<StringReplacementSlot, TResult> handleSlot);
    }

    public interface IIntegerOrSlot : IExpressionNode
    {
        TResult Handle<TResult>(Func<int, TResult> handleLiteral, Func<IntReplacementSlot, TResult> handleSlot);
    }

    public interface IDecimalOrSlot : IExpressionNode
    {
        TResult Handle<TResult>(Func<decimal, TResult> handleLiteral, Func<DecimalReplacementSlot, TResult> handleSlot);
    }

    public interface IBooleanOrSlot : IExpressionNode
    {
        TResult Handle<TResult>(Func<bool, TResult> handleLiteral, Func<BoolReplacementSlot, TResult> handleSlot);
    }

    public interface IAttributeSetOrGroupRefinement : IExpressionNode
    {
        TResult Handle<TResult>(Func<AttributeSetRefinement, TResult> handleSetRefinement, Func<AttributeGroupRefinement, TResult> handleGroupRefinement);
    }

    public class ExpressionTemplate : IExpressionNode
    {
        public ExpressionTemplate(IDefinitionStatusOrSlot defintionStatus, ISubexpressionOrSlot subexpression)
        {
            DefinitionStatus = defintionStatus ?? throw new ArgumentNullException(nameof(defintionStatus));
            Subexpression = subexpression ?? throw new ArgumentNullException(nameof(subexpression));
        }

        public IDefinitionStatusOrSlot DefinitionStatus { get; }
        public ISubexpressionOrSlot Subexpression { get; }

        public bool ContainsSlot()
        {
            return DefinitionStatus.ContainsSlot() || Subexpression.ContainsSlot(); 
        }
    }

    public class DefinitionStatusLiteral : IDefinitionStatusOrSlot
    {
        public DefinitionStatusLiteral(DefinitionStatusEnum value)
        {
            Value = value;
        }

        public DefinitionStatusEnum Value
        {
            get;
        }

        public bool ContainsSlot()
        {
            return false;
        }

        TResult IDefinitionStatusOrSlot.Handle<TResult>(
            Func<DefinitionStatusEnum, TResult> handleLiteral, 
            Func<TokenReplacementSlot, TResult> handleSlot)
        {
            return handleLiteral(Value);
        }
    }

    public class Subexpression : ISubexpressionOrSlot, IAttributeValueOrSlot
    {
        public IList<(TemplateInformationSlot info, IConceptReferenceOrSlot focus)> FocusConcepts { get; }
        public IAttributeSetOrGroupRefinement Refinement { get; }

        public Subexpression(ConceptReference concept)
        {
            if (concept == null) throw new ArgumentNullException(nameof(concept));
            FocusConcepts = new List<(TemplateInformationSlot info, IConceptReferenceOrSlot focus)> { (new TemplateInformationSlot() { SubjectContainsSlot = false }, concept) };
            Refinement = null;
        }

        public Subexpression(IList<(TemplateInformationSlot info, IConceptReferenceOrSlot focus)> focusConcepts, IAttributeSetOrGroupRefinement refinement)
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
                {
                    fc.info.SubjectContainsSlot = fc.focus.ContainsSlot();
                    return fc;
                }
            ).ToList();
            Refinement = refinement;
        }

        public bool ContainsSlot()
        {
            return FocusConcepts.Any(
                fc => fc.focus.ContainsSlot()) 
                || (Refinement?.ContainsSlot() ?? false);
        }

        TResult ISubexpressionOrSlot.Handle<TResult>(
            Func<Subexpression, TResult> handleSubexpression, 
            Func<ConceptReplacementSlot, TResult> handleConceptSlot, 
            Func<ExpressionReplacementSlot, TResult> handleExpressionSlot
            )
        {
            return handleSubexpression(this);
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleSubexpressionOrSlot(this);
        }

        public bool IsConceptReference { get =>
                Refinement == null
                && FocusConcepts.Count == 1
                && FocusConcepts.Single().focus.Handle(
                    handleConceptReference: c => true,
                    handleConceptSlot: s => false,
                    handleExpressionSlot: s => false
                ); }
        
        public ConceptReference GetConceptReference()
        {
            return IsConceptReference ? 
                FocusConcepts.Single().focus.Handle(
                    handleConceptReference: c => c,
                    handleConceptSlot: s => null,
                    handleExpressionSlot: s => null
                )
                : null;
        }
    }

    public class ConceptReference : IExpressionNode, IConceptReferenceOrSlot
    {
        public ConceptReference(ulong sctId, string term)
        {
            SctId = sctId;
            Term = term;
        }

        public ulong SctId { get; }
        public string Term { get; }

        public bool ContainsSlot()
        {
            return false;
        }

        TResult IConceptReferenceOrSlot.Handle<TResult>(
            Func<ConceptReference, TResult> handleConceptReference, 
            Func<ConceptReplacementSlot, TResult> handleConceptSlot, 
            Func<ExpressionReplacementSlot, TResult> handleExpressionSlot)
        {
            return handleConceptReference(this);
        }
    }

    public class AttributeSetRefinement : IExpressionNode, IAttributeSetOrGroupRefinement
    {
        public AttributeSetRefinement(AttributeSet attributeSet, IList<(TemplateInformationSlot info, AttributeSet group)> attributeGroups)
        {
            AttributeSet = attributeSet ?? throw new ArgumentNullException(nameof(attributeSet));
            AttributeGroups = attributeGroups?.Select(ag =>
                {
                    ag.info.SubjectContainsSlot = ag.group.ContainsSlot();
                    return ag;
                }
            )?.ToList();
        }

        public AttributeSet AttributeSet { get; }
        public IList<(TemplateInformationSlot info, AttributeSet group)> AttributeGroups { get; }

        public bool ContainsSlot()
        {
            return AttributeSet.ContainsSlot() || AttributeGroups.Any(group => group.group.ContainsSlot());
        }

        TResult IAttributeSetOrGroupRefinement.Handle<TResult>(
            Func<AttributeSetRefinement, TResult> handleSetRefinement, 
            Func<AttributeGroupRefinement, TResult> handleGroupRefinement)
        {
            return handleSetRefinement(this);
        }
    }

    public class AttributeGroupRefinement : IExpressionNode, IAttributeSetOrGroupRefinement
    {
        public AttributeGroupRefinement(IList<(TemplateInformationSlot info, AttributeSet group)> attributeGroups)
        {
            if (attributeGroups.Count == 0)
            {
                throw new ArgumentException($"{nameof(attributeGroups)} should be non-empty", nameof(attributeGroups));
            }
            AttributeGroups = attributeGroups.Select(ag =>
                {
                    ag.info.SubjectContainsSlot = ag.group.ContainsSlot();
                    return ag;
                }
            ).ToList();
        }

        public IList<(TemplateInformationSlot info, AttributeSet group)> AttributeGroups { get; }

        public bool ContainsSlot()
        {
            return AttributeGroups.Any(group => group.group.ContainsSlot());
        }

        TResult IAttributeSetOrGroupRefinement.Handle<TResult>(
            Func<AttributeSetRefinement, TResult> handleSetRefinement, 
            Func<AttributeGroupRefinement, TResult> handleGroupRefinement)
        {
            return handleGroupRefinement(this);
        }
    }

    public class AttributeSet : IExpressionNode
    {
        public AttributeSet(IList<(TemplateInformationSlot info, EtlAttribute attr)> attributes)
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
                ia => { 
                    ia.info.SubjectContainsSlot = ia.attr.ContainsSlot();
                    return ia;
                }
                ).ToList();
        }

        public IList<(TemplateInformationSlot info, EtlAttribute attr)> Attributes { get; }

        public bool ContainsSlot()
        {
            return Attributes.Any(attr => attr.attr.ContainsSlot());
        }
    }

    public class EtlAttribute : IExpressionNode
    {    public EtlAttribute(
            IConceptReferenceOrSlot attributeName,
            IAttributeValueOrSlot attributeValue
            )
        {
            AttributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            AttributeValue = attributeValue ?? throw new ArgumentNullException(nameof(attributeValue));
        }

        public IConceptReferenceOrSlot AttributeName { get; }
        public IAttributeValueOrSlot AttributeValue { get; }

        public bool ContainsSlot()
        {
            return AttributeName.ContainsSlot() || AttributeValue.ContainsSlot();
        }
    }

    public abstract class TemplateSlot : IExpressionNode
    {
        protected TemplateSlot(string slotName)
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


    public class TemplateInformationSlot : TemplateSlot
    {
        private readonly Cardinality cardinality;

        public TemplateInformationSlot(Cardinality cardinality = null, string slotName = null) : base(slotName)
        {
            this.cardinality = cardinality;
        }

        public Cardinality Cardinality 
        {
            get
            {
                return cardinality ?? (SubjectContainsSlot ? new Cardinality(1, null) : new Cardinality(1, 1));
            }
        }

        public override bool ContainsSlot()
        {
            return false;
        }

        internal bool SubjectContainsSlot
        {
            get;
            set;
        }
    }

    public class TokenReplacementSlot : TemplateSlot, IDefinitionStatusOrSlot
    {
        public TokenReplacementSlot(ICollection<DefinitionStatusEnum> slotTokens, string slotName) : base(slotName)
        {
            SlotTokens = slotTokens;
        }

        public ICollection<DefinitionStatusEnum> SlotTokens { get; }

        TResult IDefinitionStatusOrSlot.Handle<TResult>(
            Func<DefinitionStatusEnum, TResult> handleLiteral, 
            Func<TokenReplacementSlot, TResult> handleSlot)
        {
            return handleSlot(this);
        }
    }

    public class ConceptReplacementSlot : TemplateSlot, IConceptReferenceOrSlot, ISubexpressionOrSlot, IAttributeValueOrSlot
    {
        public ConceptReplacementSlot(string expressionConstraint, string slotName) : base(slotName)
        {
            ExpressionConstraint = expressionConstraint ?? "";
        }

        public string ExpressionConstraint { get; }

        TResult IConceptReferenceOrSlot.Handle<TResult>(
            Func<ConceptReference, TResult> handleConceptReference, 
            Func<ConceptReplacementSlot, TResult> handleConceptSlot, 
            Func<ExpressionReplacementSlot, TResult> handleExpressionSlot)
        {
            return handleConceptSlot(this);
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleSubexpressionOrSlot(this);
        }

        TResult ISubexpressionOrSlot.Handle<TResult>(
            Func<Subexpression, TResult> handleSubexpression, 
            Func<ConceptReplacementSlot, TResult> handleConceptSlot, 
            Func<ExpressionReplacementSlot, TResult> handleExpressionSlot)
        {
            return handleConceptSlot(this);
        }
    }

    public class ExpressionReplacementSlot : TemplateSlot, IConceptReferenceOrSlot, ISubexpressionOrSlot, IAttributeValueOrSlot
    {
        public ExpressionReplacementSlot(string expressionConstraint, string slotName) : base(slotName)
        {
            ExpressionConstraint = expressionConstraint ?? "";
        }
        public string ExpressionConstraint { get; }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot,
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleSubexpressionOrSlot(this);
        }

        TResult ISubexpressionOrSlot.Handle<TResult>(
            Func<Subexpression, TResult> handleSubexpression, 
            Func<ConceptReplacementSlot, TResult> handleConceptSlot, 
            Func<ExpressionReplacementSlot, TResult> handleExpressionSlot)
        {
            return handleExpressionSlot(this);
        }

        TResult IConceptReferenceOrSlot.Handle<TResult>(
            Func<ConceptReference, TResult> handleConceptReference, 
            Func<ConceptReplacementSlot, TResult> handleConceptSlot, 
            Func<ExpressionReplacementSlot, TResult> handleExpressionSlot)
        {
            return handleExpressionSlot(this);
        }
    }

    public class StringLiteral : IStringOrSlot, IAttributeValueOrSlot
    {
        public StringLiteral(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool ContainsSlot()
        {
            return false;
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleStringOrSlot(this);
        }

        TResult IStringOrSlot.Handle<TResult>(Func<string, TResult> handleLiteral, Func<StringReplacementSlot, TResult> handleSlot)
        {
            return handleLiteral(Value);
        }
    }

    public class StringReplacementSlot : TemplateSlot, IStringOrSlot, IAttributeValueOrSlot
    {
        public ICollection<string> Values { get; }

        public StringReplacementSlot(ICollection<string> values, string slotName) : base(slotName)
        {
            Values = values.Where(v => v != null).ToList();
        }

        TResult IStringOrSlot.Handle<TResult>(
            Func<string, TResult> handleLiteral, 
            Func<StringReplacementSlot, TResult> handleSlot)
        {
            return handleSlot(this);
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleStringOrSlot(this);
        }
    }

    public class IntLiteral : IAttributeValueOrSlot, IIntegerOrSlot
    {
        public IntLiteral(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public bool ContainsSlot()
        {
            return false;
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleIntOrSlot(this);
        }

        TResult IIntegerOrSlot.Handle<TResult>(
            Func<int, TResult> handleLiteral, 
            Func<IntReplacementSlot, TResult> handleSlot)
        {
            return handleLiteral(Value);
        }
    }
    
    
    public abstract class RangeReplacementSlot<T> : TemplateSlot
    {
        public RangeReplacementSlot(string slotName) : base(slotName)
        {
            Values = new List<T>();
            Ranges = new List<Range>();
        }

        public ICollection<T> Values { get; }

        public ICollection<Range> Ranges { get; }

        public void AddRange(bool? exclusiveMinimum = null, T minimumValue = default, bool? exclusiveMaximum = null, T maximumValue = default)
        {
            Ranges.Add(new Range(exclusiveMinimum, minimumValue, exclusiveMaximum, maximumValue));
        }
        public class Range
        {
            public Range(bool? exclusiveMinimum, T minimumValue, bool? exclusiveMaximum, T maximumValue)
            {
                ExclusiveMinimum = exclusiveMinimum;
                MinimumValue = minimumValue;
                ExclusiveMaximum = exclusiveMaximum;
                MaximumValue = maximumValue;
            }

            public bool? ExclusiveMinimum { get; }
            public T MinimumValue { get; }
            public bool? ExclusiveMaximum { get; }
            public T MaximumValue { get; }
        }
    }

    public class IntReplacementSlot : RangeReplacementSlot<int?>, IIntegerOrSlot, IAttributeValueOrSlot
    {
        public IntReplacementSlot(string slotName) : base(slotName)
        {

        }

        TResult IIntegerOrSlot.Handle<TResult>(
            Func<int, TResult> handleLiteral, 
            Func<IntReplacementSlot, TResult> handleSlot)
        {
            return handleSlot(this);
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleIntOrSlot(this);
        }
    }

    public class DecimalLiteral : IDecimalOrSlot, IAttributeValueOrSlot
    {
        public DecimalLiteral(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }

        public bool ContainsSlot()
        {
            return false;
        }

        TResult IDecimalOrSlot.Handle<TResult>(
            Func<decimal, TResult> handleLiteral, 
            Func<DecimalReplacementSlot, TResult> handleSlot)
        {
            return handleLiteral(Value);
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleDecimalOrSlot(this);
        }
    }
    public class DecimalReplacementSlot : RangeReplacementSlot<decimal?>, IDecimalOrSlot, IAttributeValueOrSlot
    {
        public DecimalReplacementSlot(string slotName) : base(slotName)
        {
        }

        TResult IDecimalOrSlot.Handle<TResult>(
            Func<decimal, TResult> handleLiteral, 
            Func<DecimalReplacementSlot, TResult> handleSlot)
        {
            return handleSlot(this);
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleDecimalOrSlot(this);
        }
    }

    public class BoolLiteral : IBooleanOrSlot, IAttributeValueOrSlot
    {
        public BoolLiteral(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        public bool ContainsSlot()
        {
            return false;
        }

        TResult IBooleanOrSlot.Handle<TResult>(
            Func<bool, TResult> handleLiteral, 
            Func<BoolReplacementSlot, TResult> handleSlot)
        {
            return handleLiteral(Value);
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleBoolOrSlot(this);
        }
    }

    public class BoolReplacementSlot : TemplateSlot, IBooleanOrSlot, IAttributeValueOrSlot
    {
        public ICollection<bool> Values { get; }

        public BoolReplacementSlot(ICollection<bool> values, string slotName) : base(slotName)
        {
            Values = values;
        }

        TResult IBooleanOrSlot.Handle<TResult>(
            Func<bool, TResult> handleLiteral, 
            Func<BoolReplacementSlot, TResult> handleSlot)
        {
            return handleSlot(this);
        }

        TResult IAttributeValueOrSlot.Handle<TResult>(
            Func<ISubexpressionOrSlot, TResult> handleSubexpressionOrSlot, 
            Func<IStringOrSlot, TResult> handleStringOrSlot, 
            Func<IIntegerOrSlot, TResult> handleIntOrSlot, 
            Func<IDecimalOrSlot, TResult> handleDecimalOrSlot, 
            Func<IBooleanOrSlot, TResult> handleBoolOrSlot)
        {
            return handleBoolOrSlot(this);
        }
    }
}
