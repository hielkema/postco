using System;

namespace SnomedTemplateService.Util
{
    public abstract class SwitchType<T1, T2>
    {
        public abstract R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2);

    }

    public class SwitchTypeCase1<T1, T2> : SwitchType<T1, T2>
    {
        public SwitchTypeCase1(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }
        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2)
        {
            return handleCase1(Value);
        }
    }

    public class SwitchTypeCase2<T1, T2> : SwitchType<T1, T2>
    {
        public SwitchTypeCase2(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2)
        {
            return handleCase2(Value);
        }
    }
    public abstract class SwitchType<T1, T2, T3>
    {
        public abstract R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3);

    }

    public class SwitchTypeCase1<T1, T2, T3> : SwitchType<T1, T2, T3>
    {
        public SwitchTypeCase1(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3)
        {
            return handleCase1(Value);
        }
    }
    public class SwitchTypeCase2<T1, T2, T3> : SwitchType<T1, T2, T3>
    {
        public SwitchTypeCase2(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3)
        {
            return handleCase2(Value);
        }
    }
    public class SwitchTypeCase3<T1, T2, T3> : SwitchType<T1, T2, T3>
    {
        public SwitchTypeCase3(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3)
        {
            return handleCase3(Value);
        }
    }

    public abstract class SwitchType<T1, T2, T3, T4>
    {
        public abstract R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4);

    }

    public class SwitchTypeCase1<T1, T2, T3, T4> : SwitchType<T1, T2, T3, T4>
    {
        public SwitchTypeCase1(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4)
        {
            return handleCase1(Value);
        }
    }

    public class SwitchTypeCase2<T1, T2, T3, T4> : SwitchType<T1, T2, T3, T4>
    {
        public SwitchTypeCase2(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4)
        {
            return handleCase2(Value);
        }
    }
    public class SwitchTypeCase3<T1, T2, T3, T4> : SwitchType<T1, T2, T3, T4>
    {
        public SwitchTypeCase3(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4)
        {
            return handleCase3(Value);
        }
    }
    public class SwitchTypeCase4<T1, T2, T3, T4> : SwitchType<T1, T2, T3, T4>
    {
        public SwitchTypeCase4(T4 value)
        {
            Value = value;
        }

        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4)
        {
            return handleCase4(Value);
        }
    }

    public abstract class SwitchType<T1, T2, T3, T4, T5>
    {
        public abstract R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5);

    }

    public class SwitchTypeCase1<T1, T2, T3, T4, T5> : SwitchType<T1, T2, T3, T4, T5>
    {
        public SwitchTypeCase1(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5)
        {
            return handleCase1(Value);
        }
    }

    public class SwitchTypeCase2<T1, T2, T3, T4, T5> : SwitchType<T1, T2, T3, T4, T5>
    {
        public SwitchTypeCase2(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5)
        {
            return handleCase2(Value);
        }
    }
    public class SwitchTypeCase3<T1, T2, T3, T4, T5> : SwitchType<T1, T2, T3, T4, T5>
    {
        public SwitchTypeCase3(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5)
        {
            return handleCase3(Value);
        }
    }
    public class SwitchTypeCase4<T1, T2, T3, T4, T5> : SwitchType<T1, T2, T3, T4, T5>
    {
        public SwitchTypeCase4(T4 value)
        {
            Value = value;
        }
        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5)
        {
            return handleCase4(Value);
        }
    }
    public class SwitchTypeCase5<T1, T2, T3, T4, T5> : SwitchType<T1, T2, T3, T4, T5>
    {
        public SwitchTypeCase5(T5 value)
        {
            Value = value;
        }

        public T5 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5)
        {
            return handleCase5(Value);
        }
    }

    public abstract class SwitchType<T1, T2, T3, T4, T5, T6>
    {
        public abstract R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6);

    }

    public class SwitchTypeCase1<T1, T2, T3, T4, T5, T6> : SwitchType<T1, T2, T3, T4, T5, T6>
    {
        public SwitchTypeCase1(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6)
        {
            return handleCase1(Value);
        }
    }

    public class SwitchTypeCase2<T1, T2, T3, T4, T5, T6> : SwitchType<T1, T2, T3, T4, T5, T6>
    {
        public SwitchTypeCase2(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6)
        {
            return handleCase2(Value);
        }
    }
    public class SwitchTypeCase3<T1, T2, T3, T4, T5, T6> : SwitchType<T1, T2, T3, T4, T5, T6>
    {
        public SwitchTypeCase3(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6)
        {
            return handleCase3(Value);
        }
    }
    public class SwitchTypeCase4<T1, T2, T3, T4, T5, T6> : SwitchType<T1, T2, T3, T4, T5, T6>
    {
        public SwitchTypeCase4(T4 value)
        {
            Value = value;
        }
        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6)
        {
            return handleCase4(Value);
        }
    }
    public class SwitchTypeCase5<T1, T2, T3, T4, T5, T6> : SwitchType<T1, T2, T3, T4, T5, T6>
    {
        public SwitchTypeCase5(T5 value)
        {
            Value = value;
        }

        public T5 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6)
        {
            return handleCase5(Value);
        }
    }

    public class SwitchTypeCase6<T1, T2, T3, T4, T5, T6> : SwitchType<T1, T2, T3, T4, T5, T6>
    {
        public SwitchTypeCase6(T6 value)
        {
            Value = value;
        }

        public T6 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6)
        {
            return handleCase6(Value);
        }
    }

    public abstract class SwitchType<T1, T2, T3, T4, T5, T6, T7>
    {
        public abstract R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7);

    }

    public class SwitchTypeCase1<T1, T2, T3, T4, T5, T6, T7> : SwitchType<T1, T2, T3, T4, T5, T6, T7>
    {
        public SwitchTypeCase1(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7)
        {
            return handleCase1(Value);
        }
    }

    public class SwitchTypeCase2<T1, T2, T3, T4, T5, T6, T7> : SwitchType<T1, T2, T3, T4, T5, T6, T7>
    {
        public SwitchTypeCase2(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7)
        {
            return handleCase2(Value);
        }
    }
    public class SwitchTypeCase3<T1, T2, T3, T4, T5, T6, T7> : SwitchType<T1, T2, T3, T4, T5, T6, T7>
    {
        public SwitchTypeCase3(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7)
        {
            return handleCase3(Value);
        }
    }
    public class SwitchTypeCase4<T1, T2, T3, T4, T5, T6, T7> : SwitchType<T1, T2, T3, T4, T5, T6, T7>
    {
        public SwitchTypeCase4(T4 value)
        {
            Value = value;
        }
        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7)
        {
            return handleCase4(Value);
        }
    }
    public class SwitchTypeCase5<T1, T2, T3, T4, T5, T6, T7> : SwitchType<T1, T2, T3, T4, T5, T6, T7>
    {
        public SwitchTypeCase5(T5 value)
        {
            Value = value;
        }

        public T5 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7)
        {
            return handleCase5(Value);
        }
    }

    public class SwitchTypeCase6<T1, T2, T3, T4, T5, T6, T7> : SwitchType<T1, T2, T3, T4, T5, T6, T7>
    {
        public SwitchTypeCase6(T6 value)
        {
            Value = value;
        }

        public T6 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7)
        {
            return handleCase6(Value);
        }
    }

    public class SwitchTypeCase7<T1, T2, T3, T4, T5, T6, T7> : SwitchType<T1, T2, T3, T4, T5, T6, T7>
    {
        public SwitchTypeCase7(T7 value)
        {
            Value = value;
        }

        public T7 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7)
        {
            return handleCase7(Value);
        }
    }

    public abstract class SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public abstract R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8);

    }

    public class SwitchTypeCase1<T1, T2, T3, T4, T5, T6, T7, T8> : SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SwitchTypeCase1(T1 value)
        {
            Value = value;
        }
        public T1 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8)
        {
            return handleCase1(Value);
        }
    }

    public class SwitchTypeCase2<T1, T2, T3, T4, T5, T6, T7, T8> : SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SwitchTypeCase2(T2 value)
        {
            Value = value;
        }
        public T2 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8)
        {
            return handleCase2(Value);
        }
    }
    public class SwitchTypeCase3<T1, T2, T3, T4, T5, T6, T7, T8> : SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SwitchTypeCase3(T3 value)
        {
            Value = value;
        }
        public T3 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8)
        {
            return handleCase3(Value);
        }
    }
    public class SwitchTypeCase4<T1, T2, T3, T4, T5, T6, T7, T8> : SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SwitchTypeCase4(T4 value)
        {
            Value = value;
        }
        public T4 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8)
        {
            return handleCase4(Value);
        }
    }
    public class SwitchTypeCase5<T1, T2, T3, T4, T5, T6, T7, T8> : SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SwitchTypeCase5(T5 value)
        {
            Value = value;
        }

        public T5 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8)
        {
            return handleCase5(Value);
        }
    }

    public class SwitchTypeCase6<T1, T2, T3, T4, T5, T6, T7, T8> : SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SwitchTypeCase6(T6 value)
        {
            Value = value;
        }

        public T6 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8)
        {
            return handleCase6(Value);
        }
    }

    public class SwitchTypeCase7<T1, T2, T3, T4, T5, T6, T7, T8> : SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SwitchTypeCase7(T7 value)
        {
            Value = value;
        }

        public T7 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8)
        {
            return handleCase7(Value);
        }
    }

    public class SwitchTypeCase8<T1, T2, T3, T4, T5, T6, T7, T8> : SwitchType<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        public SwitchTypeCase8(T8 value)
        {
            Value = value;
        }

        public T8 Value { get; }

        public override R Handle<R>(Func<T1, R> handleCase1, Func<T2, R> handleCase2, Func<T3, R> handleCase3, Func<T4, R> handleCase4, Func<T5, R> handleCase5, Func<T6, R> handleCase6, Func<T7, R> handleCase7, Func<T8, R> handleCase8)
        {
            return handleCase8(Value);
        }
    }
}
