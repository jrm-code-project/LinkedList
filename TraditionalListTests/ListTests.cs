using AdtList;
using CommonLispLinkedLists;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ListTests
{
    [TestClass]
    public class ListTests
    {
        [TestMethod]
        public void TestCons ()
        {
            Cons testCons = new Cons (2, 3);
            Assert.AreEqual (2, testCons.Car);
            Assert.AreEqual (3, testCons.Cdr);
        }

        [TestMethod]
        public void TestConsP ()
        {
            Assert.IsTrue (CommonLisp.ConsP (new Cons (2, 3)));
            Assert.IsFalse (CommonLisp.ConsP ("foobar"));
            Assert.IsFalse (CommonLisp.ConsP (null));

            // The empty list is not null.
            Assert.IsNotNull (List.Empty);
            // Lists are not cons cells.
            Assert.IsFalse (CommonLisp.ConsP (List.Empty.Cons ("a list")));
        }

        [TestMethod]
        public void TestCar ()
        {
            Assert.IsNull (CommonLisp.Car (null));
            Assert.AreEqual (2, CommonLisp.Car (new Cons (2, 3)));
            Assert.ThrowsException<ArgumentException> (() => CommonLisp.Car ("not a list"));
        }

        [TestMethod]
        public void TestCdr ()
        {
            Assert.IsNull (CommonLisp.Cdr (null));
            Assert.AreEqual (3, CommonLisp.Cdr (new Cons (2, 3)));
            Assert.ThrowsException<ArgumentException> (() => CommonLisp.Cdr ("not a list"));
        }

        [TestMethod]
        public void TestEndP ()
        {
            Assert.IsTrue (CommonLisp.EndP (null));
            Assert.IsFalse (CommonLisp.EndP (new Cons (2, 3)));
            Assert.ThrowsException<ArgumentException> (() => CommonLisp.EndP ("not a list"));

            Assert.IsTrue (AdtLisp.TheEmptyList.EndP);
            Assert.IsFalse (AdtLisp.TheEmptyList.Cons (1).EndP);
        }

        [TestMethod]
        public void TestListP ()
        {
            Assert.IsTrue (CommonLisp.ListP (new Cons ("a list", null)));
            Assert.IsTrue (CommonLisp.ListP (null));
            Assert.IsTrue (CommonLisp.ListP (new Cons ("a dotted list", 3)));

            // The following block of assertions are trivially correct, so the compiler
            // issues warnings.  We just muffle them here.
#pragma warning disable CS0183, CS0184

            // Null is not an empty list.
            Assert.IsFalse (null is List);
            // Nor are arbitrary Cons cells, even if they cons on to null.
            Assert.IsFalse (new Cons ("not a list", null) is List);
            // There is one distinguished empty list.  All lists end with it.
            Assert.IsTrue (List.Empty is List);
            // One way to make a new list is by Consing an element to an existing list.
            // But Cons only works on existing lists and cannot create dotted lists.
            Assert.IsTrue (List.Empty.Cons ("a list") is List);
            // Must use "object-oriented" syntax.
            Assert.IsFalse (new Cons ("not a list", List.Empty) is List);

#pragma warning restore
        }

        private List MakeTestList ()
        {
            return AdtLisp.List (1, 2, 3);
        }

        [TestMethod]
        public void TestListAccessors ()
        {
            List testList = MakeTestList ();
            Assert.AreEqual (1, testList.First ());
            List tail1 = testList.Rest ();
            Assert.AreEqual (2, tail1.First ());
            List tail2 = tail1.Rest ();
            Assert.AreEqual (3, tail2.First ());
            List tail3 = tail2.Rest ();
            Assert.IsTrue (tail3.EndP);
            Assert.ThrowsException<ArgumentException> (() => tail3.First ());
            Assert.ThrowsException<ArgumentException> (() => tail3.Rest ());
        }

        [TestMethod]
        public void TestRevappend ()
        {
            List head = AdtLisp.List (1, 2, 3);
            List tail = AdtLisp.List (4, 5, 6);
            List result = AdtLisp.Revappend (head, tail);
            Assert.AreEqual (3, result.First ());
            List tail1 = result.Rest ();
            Assert.AreEqual (2, tail1.First ());
            List tail2 = tail1.Rest ();
            Assert.AreEqual (1, tail2.First ());
            List tail3 = tail2.Rest ();
            Assert.AreEqual (4, tail3.First ());
            List tail4 = tail3.Rest ();
            Assert.AreEqual (5, tail4.First ());
            List tail5 = tail4.Rest ();
            Assert.AreEqual (6, tail5.First ());
            List tail6 = tail5.Rest ();
            Assert.IsTrue (tail6.EndP);
        }
    }
}
