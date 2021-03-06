﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace AdtList
{
    /// <summary>
    /// A List is a struct rather than a class, but there is only one field in a list, which is
    /// a reference to the head ListCell.  Thus a List is only "logically" a wrapper around the
    /// first cell instead of a pointer to it.  This avoids the needless indirection that would
    /// result if List were a class.
    /// </summary>
    public struct List : IEnumerable
    {
        /// <summary>
        /// A ListCell is the representation of a List.  It differs from a Cons in 2 ways.
        /// <list type="number">
        /// <item>It is immutable, thus Lists are immutable.</item>
        /// <item>The CDR (rest) is a List, not an object or another cell. (i.e. the representation
        ///     of a list is simply the head cell, not the entire spine.)</item>
        /// </list>
        /// </summary>
        private class ListCell
        {
            /// <summary>
            /// The field that holds the first element of the list.
            /// </summary>
            [DebuggerBrowsableAttribute (DebuggerBrowsableState.Never)]
            private readonly object first;

            /// <summary>
            /// The field that holds the remaining elements of a list after the first.
            /// </summary>
            [DebuggerBrowsableAttribute (DebuggerBrowsableState.Never)]
            private readonly List rest;

            /// <summary>
            /// Constructs the concrete representation of a List.  This should eventually be turned into
            /// an abstract list by calling new List() on it.
            /// </summary>
            /// <param name="first">An object that will be the first element of the list.</param>
            /// <param name="rest">A (possibly empty) List that will be the remainder of the list.</param>
            public ListCell (object first, List rest)
            {
                this.first = first;
                this.rest = rest;
            }

            /// <summary>
            /// A property that returns the first element of the ListCell.
            /// </summary>
            public object First => first;

            /// <summary>
            /// A property that returns the remaining elements of the List cell.
            /// </summary>
            public List Rest => rest;

            public override string ToString ()
            {
                return "(" +
                    (first is null ? "null" : first.ToString ()) +
                    " . " +
                    rest.ToString () +
                    ")";
            }
        }

        /// <summary>
        /// Field that refers to the ListCell that represents the head of the list, or null if the list is empty.
        /// Since List is a struct, this field refers to where the pointer to the ListCell will be stored.
        /// </summary>
        private readonly ListCell headCell;

        /// <summary>
        /// Private constructor turns representation into abstract object.
        /// But since List is a struct, this constructor doesn't actually
        /// do anything but copy the pointer.
        /// </summary>
        /// <param name="headCell">The concrete representation of the list.</param>
        private List (ListCell headCell) { this.headCell = headCell; }

        /// <summary>A distinguised empty list that all lists end with.  It's just a list
        /// with a null headCell.</summary>
        public static List Empty => theEmptyList;

        private static readonly List theEmptyList = new List (null);

        /// <summary>
        /// The standard way of constructing a new list by prepending a single element on to
        /// the front of an existing list.
        /// </summary>
        /// <param name="newHead">The element to be prepended to the list.</param>
        /// <returns>A new list with <paramref name="newHead"/> prepended to it.</returns>
        public List Cons (object newHead)
        {
            return new List (new ListCell (newHead, this));
        }

        /// <summary>
        /// Determines whether the list is the empty list.
        /// </summary>
        /// <returns><see langword="true"/>If list is the empty list.</returns>
        public bool EndP => headCell is null;

        /// <summary>
        /// Primitive accessor returns the first element of a list.  <paramref name="this" /> must not be the empty list.
        /// </summary>
        /// <returns>The first element of a non-empty list.</returns>
        public object First ()
        {
            if (EndP)
                throw new ArgumentException (nameof (First) + " called on an empty list.");
            else
                return headCell.First;
        }

        /// <summary>
        /// Primitive accessor that returns all but the first element of a list.  <paramref name="this" />
        /// must not be the empty list.
        /// </summary>
        /// <returns>The remainder of <paramref name="this" /> with the first element removed.  Result
        /// will be a (possibly empty) list.</returns>
        public List Rest ()
        {
            if (EndP)
                throw new ArgumentException (nameof (Rest) + " called on an empty list.");
            else
                return headCell.Rest;
        }

        // Override appropriate methods for value types (structs).
        public override bool Equals (object other)
        {
            return other is null ? false
                : Object.ReferenceEquals (other, this) ? true
                : other is List oList ? oList.headCell == headCell
                : false;
        }

        public static bool operator == (List left, object right) => left.Equals (right);
        public static bool operator == (object left, List right) => right.Equals (left);

        public static bool operator != (List left, object right) => !left.Equals (right);
        public static bool operator != (object left, List right) => !right.Equals (left);

        public override int GetHashCode ()
        {
            return headCell is null ? 0 : headCell.First.GetHashCode ();
        }

        // Overly complex ToString method handles limits on Depth
        // and Length set by PrintLength and PrintDepth.
        public static int PrintLength { get; set; } = 10;
        public static int PrintDepth { get; set; } = 3;

        public override string ToString ()
        {
            if (PrintDepth == 0) return "#";
            if (EndP) return "()";
            StringBuilder sb = new StringBuilder ();
            sb.Append ("(");
            if (PrintLength == 0)
            {
                sb.Append ("...");
            }
            else
            {

                int oldPrintDepth = PrintDepth;
                try
                {
                    PrintDepth -= 1;
                    sb.Append (headCell.First.ToString ());
                }
                finally
                {
                    PrintDepth = oldPrintDepth;
                }

            }
            int count = 1;
            List tail = headCell.Rest;
            while (true)
            {
                if (tail.EndP) break;
                if (count >= PrintLength)
                {
                    sb.Append (" ...");
                    break;
                }
                else
                {
                    sb.Append (" ");
                    int oldPrintDepth = PrintDepth;
                    try
                    {
                        PrintDepth -= 1;
                        sb.Append (tail.First ().ToString ());
                    }
                    finally
                    {
                        PrintDepth = oldPrintDepth;
                    }
                    count += 1;
                    tail = tail.Rest ();
                }
            }
            sb.Append (")");
            return sb.ToString ();
        }

        public IEnumerator GetEnumerator ()
        {
            return new ListEnumerator (this);
        }
    }

    public class ListEnumerator : IEnumerator
    {
        private List head;
        private List current;
        private bool afterEnd;

        public ListEnumerator (List head)
        {
            this.head = head;
        }

        public object Current
        {
            get
            {
                if (head.EndP || current.EndP)
                {
                    throw new InvalidOperationException ();
                }
                return current.First ();
            }
        }

        public bool MoveNext ()
        {
            if (afterEnd)
                throw new InvalidOperationException ();

            current = current.EndP ? head : current.Rest ();
            if (current.EndP)
            {
                afterEnd = true;
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Reset ()
        {
            current = List.Empty;
            afterEnd = false;
        }

        // Implement IDisposable, which is also implemented by IEnumerator(T).
        private bool disposedValue = false;
        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose of managed resources.
                }
                current = List.Empty;
                if (!head.EndP)
                {
                    head = List.Empty;
                }
            }

            disposedValue = true;
        }

        ~ListEnumerator ()
        {
            Dispose (false);
        }
    }

    public static class AdtLisp
    {
        // Car (list):  Instead of calling Car on a list, use the object-oriented syntax of list.First()
        [Obsolete ("Use 'list.First()' instead.")]
        public static object Car (List list)
        {
            return list.First ();
        }

        [Obsolete ("Use 'list.First()' instead.")]
        public static object Car (object o)
        {
            return ((List) o).First ();
        }

        // Cdr (list):  Instead of calling Cdr on a list, use the object-oriented syntax of list.Rest()
        [Obsolete ("Use 'list.Rest()' instead.")]
        public static List Cdr (List list)
        {
            return list.Rest ();
        }

        [Obsolete ("Use 'list.Rest()' instead.")]
        public static List Cdr (object o)
        {
            return ((List) o).Rest ();
        }

        // Consp (val):  Use 'val is Cons' instead.  Does not return true for lists.
        [Obsolete ("Use 'obj is Cons' instead.")]
        public static bool ConsP (object o)
        {
            return o is CommonLispLinkedLists.Cons;
        }

        public static bool EndP (List list)
        {
            return list.EndP;
        }

        public static bool EndP (object o)
        {
            return ((List) o).EndP;
        }

        public static List List (params object[] elements)
        {
            return VectorToList (elements);
        }

        [Obsolete ("Use 'obj is List' instead.")]
        public static bool ListP (object o)
        {
            return o is List;
        }

        public static List Revappend (List list, List tail)
        {
            List answer = tail;
            foreach (object o in list)
            {
                answer = answer.Cons (o);
            }
            return answer;
        }

        public static List SubvectorToList (object[] vector, int start, int end)
        {
            List answer = TheEmptyList;
            for (int index = end - 1; index >= start; --index)
            {
                answer = answer.Cons (vector[index]);
            }
            return answer;
        }

        public static readonly List TheEmptyList = AdtList.List.Empty;

        public static List VectorToList (object[] vector)
        {
            return SubvectorToList (vector, 0, vector.Length);
        }
    }
}
