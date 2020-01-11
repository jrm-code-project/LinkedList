using System;
using System.Diagnostics;

namespace AdtList
{
    /// <summary>
    /// A List is a struct rather than a class, but there is only one field in a list, which is
    /// a reference to the head ListCell.  Thus a List is only "logically" a wrapper around the
    /// first cell instead of a pointer to it.  This avoids the needless indirection that would
    /// result if List were a class.
    /// </summary>
    public struct List
    {
        /// <summary>
        /// A ListCell is the representation of a List.  It differs from a Cons in 2 ways.
        /// <list type="number">
        /// <item>It is immutable, thus Lists are immutable.</item>
        /// <item>The CDR (rest) is a List, not an object or another cell. (i.e. the representation
        ///     of a list is simply the head cell, not the entire spine.)</item>
        /// </list>
        /// </summary>
        class ListCell
        {
            /// <summary>
            /// The field that holds the first element of the list.
            /// </summary>
            [DebuggerBrowsableAttribute (DebuggerBrowsableState.Never)]
            readonly object first;

            /// <summary>
            /// The field that holds the remaining elements of a list after the first.
            /// </summary>
            [DebuggerBrowsableAttribute (DebuggerBrowsableState.Never)]
            readonly List rest;

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
        }

        /// <summary>
        /// Field that refers to the ListCell that represents the head of the list, or null if the list is empty.
        /// Since List is a struct, this field refers to where the pointer to the ListCell will be stored.
        /// </summary>
        readonly ListCell headCell;

        /// <summary>
        /// Private constructor turns representation into abstract object.
        /// But since List is a struct, this constructor doesn't actually
        /// do anything but copy the pointer.
        /// </summary>
        /// <param name="headCell">The concrete representation of the list.</param>
        List (ListCell headCell) { this.headCell = headCell; }

        /// <summary>A distinguised empty list that all lists end with.  It's just a list
        /// with a null headCell.</summary>
        public static List Empty => theEmptyList;
        static readonly List theEmptyList = new List (null);

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
        /// Primitive accessor returns the first element of a list.  <paramref name="this" /> must not be the empty list.
        /// </summary>
        /// <returns>The first element of a non-empty list.</returns>
        public object First ()
        {
            if (headCell is null)
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
            if (headCell is null)
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
    }

    public static class AdtLisp
    {
        // Car (list):  Instead of calling Car on a list, use the object-oriented syntax of list.First()
        [Obsolete ("Use 'list.First()' instead.")]
        public static object Car (List list) => list.First ();

        [Obsolete ("Use 'list.First()' instead.")]
        public static object Car (object o) => ((List) o).First ();

        // Cdr (list):  Instead of calling Cdr on a list, use the object-oriented syntax of list.Rest()
        [Obsolete ("Use 'list.Rest()' instead.")]
        public static List Cdr (List list) => list.Rest ();

        [Obsolete ("Use 'list.Rest()' instead.")]
        public static List Cdr (object o) => ((List) o).Rest ();

        // Consp (val):  Use 'val is Cons' instead.  Does not return true for lists.
        [Obsolete ("Use 'obj is Cons' instead.")]
        public static bool ConsP (object o) => o is CommonLispLinkedLists.Cons;

        public static bool EndP (object o) => Object.ReferenceEquals (o, TheEmptyList);

        public static List List (params object [] elements)
        {
            return VectorToList (elements);
        }

        [Obsolete ("Use 'obj is List' instead.")]
        public static bool ListP (object o) => o is List;

        public static List SubvectorToList (object [] vector, int start, int end)
        {
            List answer = TheEmptyList;
            for (int index = end - 1; index >= start; --index)
            {
                answer = answer.Cons (vector[index]);
            }
            return answer;
        }

        static readonly List TheEmptyList = AdtList.List.Empty;

        public static List VectorToList (object [] vector)
        {
            return SubvectorToList (vector, 0, vector.Length);
        }
    }
}
