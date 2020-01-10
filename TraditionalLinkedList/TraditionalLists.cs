using System;
using System.Diagnostics;

namespace CommonLispLinkedLists
{
    public class Cons
    {
        [DebuggerBrowsable (DebuggerBrowsableState.Never)]
        object car;
        [DebuggerBrowsable (DebuggerBrowsableState.Never)]
        object cdr;

        public Cons (object car, object cdr)
        {
            this.car = car;
            this.cdr = cdr;
        }

        public object Car
        {
            get => car;
            set => car = value;
        }

        public object Cdr
        {
            get => cdr;
            set => cdr = value;
        }
    }

    public static class CommonLisp
    {
        public static object Car (object o)
        {
            return (o is null) ? null
                 : (o is Cons oCons) ? oCons.Car
                 : throw new ArgumentException (nameof (Car) + ": Wrong type argument", nameof (o));
        }

        public static object Cdr (object o)
        {
            return (o is null) ? null
                : (o is Cons oCons) ? oCons.Cdr
                : throw new ArgumentException (nameof (Cdr) + ": Wrong type argument", nameof (o));
        }

        public static bool ConsP (object o)
        {
            return o is Cons;
        }

        public static bool EndP (object o)
        {
            return (o is null) ? true
                : (o is Cons) ? false
                : throw new ArgumentException (nameof (EndP) + ": Wrong type argument:", nameof (o));
        }

        public static bool ListP (object o)
        {
            return o is null || o is Cons;
        }
    }
}
