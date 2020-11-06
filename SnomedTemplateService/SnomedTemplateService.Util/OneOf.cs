using System;

namespace SnomedTemplateService.Util
{
    // Sum type for .NET (see: https://en.wikipedia.org/wiki/Tagged_union)

    public abstract class OneOf<T1, T2>
    {
        public abstract R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond);

    }

    public class FirstOf<T1, T2> : OneOf<T1, T2>
    {
        public FirstOf(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }
        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond)
        {
            return handleFirst(Value);
        }
    }

    public class SecondOf<T1, T2> : OneOf<T1, T2>
    {
        public SecondOf(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond)
        {
            return handleSecond(Value);
        }
    }
    public abstract class OneOf<T1, T2, T3>
    {
        public abstract R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird);

    }

    public class FirstOf<T1, T2, T3> : OneOf<T1, T2, T3>
    {
        public FirstOf(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird)
        {
            return handleFirst(Value);
        }
    }
    public class SecondOf<T1, T2, T3> : OneOf<T1, T2, T3>
    {
        public SecondOf(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird)
        {
            return handleSecond(Value);
        }
    }
    public class ThirdOf<T1, T2, T3> : OneOf<T1, T2, T3>
    {
        public ThirdOf(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird)
        {
            return handleThird(Value);
        }
    }

    public abstract class OneOf<T1, T2, T3, T4>
    {
        public abstract R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth);

    }

    public class FirstOf<T1, T2, T3, T4> : OneOf<T1, T2, T3, T4>
    {
        public FirstOf(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth)
        {
            return handleFirst(Value);
        }
    }

    public class SecondOf<T1, T2, T3, T4> : OneOf<T1, T2, T3, T4>
    {
        public SecondOf(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth)
        {
            return handleSecond(Value);
        }
    }
    public class ThirdOf<T1, T2, T3, T4> : OneOf<T1, T2, T3, T4>
    {
        public ThirdOf(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth)
        {
            return handleThird(Value);
        }
    }
    public class FourthOf<T1, T2, T3, T4> : OneOf<T1, T2, T3, T4>
    {
        public FourthOf(T4 value)
        {
            Value = value;
        }

        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth)
        {
            return handleFourth(Value);
        }
    }

    public abstract class OneOf<T1, T2, T3, T4, T5>
    {
        public abstract R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth);

    }

    public class FirstOf<T1, T2, T3, T4, T5> : OneOf<T1, T2, T3, T4, T5>
    {
        public FirstOf(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth)
        {
            return handleFirst(Value);
        }
    }

    public class SecondOf<T1, T2, T3, T4, T5> : OneOf<T1, T2, T3, T4, T5>
    {
        public SecondOf(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth)
        {
            return handleSecond(Value);
        }
    }
    public class ThirdOf<T1, T2, T3, T4, T5> : OneOf<T1, T2, T3, T4, T5>
    {
        public ThirdOf(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth)
        {
            return handleThird(Value);
        }
    }
    public class FourthOf<T1, T2, T3, T4, T5> : OneOf<T1, T2, T3, T4, T5>
    {
        public FourthOf(T4 value)
        {
            Value = value;
        }
        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth)
        {
            return handleFourth(Value);
        }
    }
    public class FifthOf<T1, T2, T3, T4, T5> : OneOf<T1, T2, T3, T4, T5>
    {
        public FifthOf(T5 value)
        {
            Value = value;
        }

        public T5 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth)
        {
            return handleFifth(Value);
        }
    }

    public abstract class OneOf<T1, T2, T3, T4, T5, T6>
    {
        public abstract R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth);

    }

    public class FirstOf<T1, T2, T3, T4, T5, T6> : OneOf<T1, T2, T3, T4, T5, T6>
    {
        public FirstOf(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth)
        {
            return handleFirst(Value);
        }
    }

    public class SecondOf<T1, T2, T3, T4, T5, T6> : OneOf<T1, T2, T3, T4, T5, T6>
    {
        public SecondOf(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth)
        {
            return handleSecond(Value);
        }
    }
    public class ThirdOf<T1, T2, T3, T4, T5, T6> : OneOf<T1, T2, T3, T4, T5, T6>
    {
        public ThirdOf(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth)
        {
            return handleThird(Value);
        }
    }
    public class FourthOf<T1, T2, T3, T4, T5, T6> : OneOf<T1, T2, T3, T4, T5, T6>
    {
        public FourthOf(T4 value)
        {
            Value = value;
        }
        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth)
        {
            return handleFourth(Value);
        }
    }
    public class FifthOf<T1, T2, T3, T4, T5, T6> : OneOf<T1, T2, T3, T4, T5, T6>
    {
        public FifthOf(T5 value)
        {
            Value = value;
        }

        public T5 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth)
        {
            return handleFifth(Value);
        }
    }

    public class SixthOf<T1, T2, T3, T4, T5, T6> : OneOf<T1, T2, T3, T4, T5, T6>
    {
        public SixthOf(T6 value)
        {
            Value = value;
        }

        public T6 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth)
        {
            return handleSixth(Value);
        }
    }

    public abstract class OneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        public abstract R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh);

    }

    public class FirstOf<T1, T2, T3, T4, T5, T6, T7> : OneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        public FirstOf(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh)
        {
            return handleFirst(Value);
        }
    }

    public class SecondOf<T1, T2, T3, T4, T5, T6, T7> : OneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        public SecondOf(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh)
        {
            return handleSecond(Value);
        }
    }
    public class ThirdOf<T1, T2, T3, T4, T5, T6, T7> : OneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        public ThirdOf(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh)
        {
            return handleThird(Value);
        }
    }
    public class FourthOf<T1, T2, T3, T4, T5, T6, T7> : OneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        public FourthOf(T4 value)
        {
            Value = value;
        }
        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh)
        {
            return handleFourth(Value);
        }
    }
    public class FifthOf<T1, T2, T3, T4, T5, T6, T7> : OneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        public FifthOf(T5 value)
        {
            Value = value;
        }

        public T5 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh)
        {
            return handleFifth(Value);
        }
    }

    public class SixthOf<T1, T2, T3, T4, T5, T6, T7> : OneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        public SixthOf(T6 value)
        {
            Value = value;
        }

        public T6 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh)
        {
            return handleSixth(Value);
        }
    }

    public class SeventhOf<T1, T2, T3, T4, T5, T6, T7> : OneOf<T1, T2, T3, T4, T5, T6, T7>
    {
        public SeventhOf(T7 value)
        {
            Value = value;
        }

        public T7 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh)
        {
            return handleSeventh(Value);
        }
    }

    public abstract class OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public abstract R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth);

    }

    public class FirstOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public FirstOf(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth)
        {
            return handleFirst(Value);
        }
    }

    public class SecondOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SecondOf(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth)
        {
            return handleSecond(Value);
        }
    }
    public class ThirdOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public ThirdOf(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth)
        {
            return handleThird(Value);
        }
    }
    public class FourthOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public FourthOf(T4 value)
        {
            Value = value;
        }
        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth)
        {
            return handleFourth(Value);
        }
    }
    public class FifthOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public FifthOf(T5 value)
        {
            Value = value;
        }

        public T5 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth)
        {
            return handleFifth(Value);
        }
    }

    public class SixthOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SixthOf(T6 value)
        {
            Value = value;
        }

        public T6 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth)
        {
            return handleSixth(Value);
        }
    }

    public class SeventhOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SeventhOf(T7 value)
        {
            Value = value;
        }

        public T7 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth)
        {
            return handleSeventh(Value);
        }
    }

    public class EighthOf<T1, T2, T3, T4, T5, T6, T7, T8> : OneOf<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public EighthOf(T8 value)
        {
            Value = value;
        }

        public T8 Value { get; }

        public override R Handle<R>(Func<T1, R> handleFirst, Func<T2, R> handleSecond, Func<T3, R> handleThird, Func<T4, R> handleFourth, Func<T5, R> handleFifth, Func<T6, R> handleSixth, Func<T7, R> handleSeventh, Func<T8, R> handleEighth)
        {
            return handleEighth(Value);
        }
    }
}
