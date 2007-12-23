using System;
using System.Collections.Generic;
using Xunit;

namespace ObjectBuilder
{
    public class LocatorTest
    {
        [Fact]
        public void AddingToSameKeyTwiceThrows()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo1", o);

            Assert.Throws<ArgumentException>(
                delegate
                {
                    locator.Add("foo1", o);
                });
        }

        [Fact]
        public void AskingForAnUnregisterdObjectReturnsNull()
        {
            IReadWriteLocator locator = new Locator();

            Assert.Null(locator.Get("Foo"));
        }

        [Fact]
        public void AskingParentStopsAsSoonAsWeFindAMatch()
        {
            object o1 = new object();
            object o2 = new object();
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);
            IReadWriteLocator grandchildLocator = new Locator(childLocator);

            rootLocator.Add("foo", o1);
            childLocator.Add("foo", o2);

            object result = grandchildLocator.Get("foo");

            Assert.NotNull(result);
            Assert.Same(o2, result);
        }

        [Fact]
        public void CanAddSameObjectTwiceWithDifferentKeys()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo1", o);
            locator.Add("foo2", o);

            Assert.Same(locator.Get("foo1"), locator.Get("foo2"));
        }

        [Fact]
        public void CanCallContainsThroughParent()
        {
            object o = new object();
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);

            rootLocator.Add("froz", o);

            Assert.True(childLocator.Contains("froz"));
        }

        [Fact]
        public void CanEnumerate()
        {
            object o1 = new object();
            object o2 = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo1", o1);
            locator.Add("foo2", o2);

            foreach (KeyValuePair<object, object> kvp in locator)
            {
                Assert.NotNull(kvp);
                Assert.NotNull(kvp.Key);
                Assert.NotNull(kvp.Value);
            }
        }

        [Fact]
        public void CanFindByPredicate()
        {
            bool wasCalled = false;
            object o1 = new object();
            object o2 = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo1", o1);
            locator.Add("foo2", o2);

            IReadableLocator results;

            results = locator.FindBy(
                delegate(KeyValuePair<object, object> kvp)
                {
                    wasCalled = true;
                    return kvp.Key.Equals("foo1");
                });

            Assert.NotNull(results);
            Assert.True(wasCalled);
            Assert.Equal(1, results.Count);
            Assert.Same(o1, results.Get("foo1"));
        }

        [Fact]
        public void CanFindOutIfContainsAKey()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo", o);
            Assert.True(locator.Contains("foo"));
            Assert.False(locator.Contains("foo2"));
        }

        [Fact]
        public void CanRegisterObjectByName()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("Bar", o);
            Assert.NotNull(locator.Get("Bar"));
            Assert.Same(o, locator.Get("Bar"));
        }

        [Fact]
        public void CanRegisterObjectByType()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add(typeof(object), o);
            Assert.NotNull(locator.Get<object>());
            Assert.Same(o, locator.Get<object>());
        }

        [Fact]
        public void CanRegisterObjectByTypeAndID()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();
            NamedTypeBuildKey key = new NamedTypeBuildKey(typeof(object), "foo");

            locator.Add(key, o);
            Assert.NotNull(locator.Get(key));
            Assert.Same(o, locator.Get(key));
        }

        [Fact]
        public void CanRegisterTwoObjectsWithDifferentKeys()
        {
            object o1 = new object();
            object o2 = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo1", o1);
            locator.Add("foo2", o2);

            Assert.Same(o1, locator.Get("foo1"));
            Assert.Same(o2, locator.Get("foo2"));
        }

        [Fact]
        public void CountReturnsNumberOfKeysWithLiveValues()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo1", o);
            locator.Add("foo2", o);

            Assert.Equal(2, locator.Count);

            o = null;
            GC.Collect();

            Assert.Equal(0, locator.Count);
        }

        [Fact]
        public void DefaultBehaviorIsAskingParent()
        {
            object o = new object();
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);

            rootLocator.Add("fiz", o);

            Assert.NotNull(childLocator.Get("fiz"));
        }

        [Fact]
        public void DefaultFindByBehaviorIsAskParent()
        {
            object o = new object();
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);

            rootLocator.Add("foo", o);

            IReadableLocator results;

            results = childLocator.FindBy(delegate
                                          {
                                              return true;
                                          });

            Assert.Equal(1, results.Count);
        }

        [Fact]
        public void FindingByPredicateCanClimbTheTree()
        {
            object o = new object();
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);

            rootLocator.Add("bar", o);

            IReadableLocator results;

            results = childLocator.FindBy(delegate
                                          {
                                              return true;
                                          });

            Assert.Equal(1, results.Count);
            Assert.Same(o, results.Get("bar"));
        }

        [Fact]
        public void FindingByPredicateCanFindsResultsFromBothParentAndChild()
        {
            object o = new object();
            string s = "Hello world";
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);

            rootLocator.Add("foo", o);
            childLocator.Add("bar", s);

            IReadableLocator results;

            results = childLocator.FindBy(delegate
                                          {
                                              return true;
                                          });

            Assert.Equal(2, results.Count);
            Assert.Same(o, results.Get("foo"));
            Assert.Same(s, results.Get("bar"));
        }

        [Fact]
        public void FindingByPredicateReturnsClosestResultsOnDuplicateKey()
        {
            object o1 = new object();
            object o2 = new object();
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);

            rootLocator.Add("foo", o1);
            childLocator.Add("foo", o2);

            IReadableLocator results;

            results = childLocator.FindBy(delegate
                                          {
                                              return true;
                                          });

            Assert.Equal(1, results.Count);
            Assert.Same(o2, results.Get("foo"));
        }

        [Fact]
        public void GetCanAskParentLocatorForAnObject()
        {
            object o = new object();
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);

            rootLocator.Add("Foo", o);
            object result = childLocator.Get("Foo");

            Assert.NotNull(result);
            Assert.Same(o, result);
        }

        [Fact]
        public void LocatorIsNotReadOnly()
        {
            IReadWriteLocator locator = new Locator();

            Assert.False(locator.ReadOnly);
        }

        [Fact]
        public void NullKeyOnAddThrows()
        {
            IReadWriteLocator locator = new Locator();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    locator.Add(null, 1);
                });
        }

        [Fact]
        public void NullKeyOnContainsThrows()
        {
            IReadWriteLocator locator = new Locator();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    locator.Contains(null);
                });
        }

        [Fact]
        public void NullKeyOnGetThrows()
        {
            IReadWriteLocator locator = new Locator();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    locator.Get(null);
                });
        }

        [Fact]
        public void NullKeyOnRemoveThrows()
        {
            IReadWriteLocator locator = new Locator();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    locator.Remove(null);
                });
        }

        [Fact]
        public void NullPredicateOnFindByThrows()
        {
            IReadWriteLocator locator = new Locator();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    locator.FindBy(null);
                });
        }

        [Fact]
        public void NullValueOnAddThrows()
        {
            IReadWriteLocator locator = new Locator();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    locator.Add(1, null);
                });
        }

        [Fact]
        public void RegisteringAnObjectWithTwoKeysAndRemovingOneKeyLeavesTheOtherOneInTheLocator()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo1", o);
            locator.Add("foo2", o);
            locator.Remove("foo1");

            Assert.Same(o, locator.Get("foo2"));
        }

        [Fact]
        public void RegistrationDoesNotPreventGarbageCollection()
        {
            IReadWriteLocator locator = new Locator();

            locator.Add("foo", new object());
            GC.Collect();

            Assert.Null(locator.Get("foo"));
        }

        [Fact]
        public void RemovingANonExistantKeyDoesntThrow()
        {
            IReadWriteLocator locator = new Locator();

            Assert.False(locator.Remove("Baz"));
        }

        [Fact]
        public void RemovingOneObjectDoesntAffectOtherObjects()
        {
            object o1 = new object();
            object o2 = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("foo1", o1);
            locator.Add("foo2", o2);

            Assert.True(locator.Remove("foo1"));
            Assert.Same(o2, locator.Get("foo2"));
        }

        [Fact]
        public void RetrievingARemovedObjectReturnsNull()
        {
            object o = new object();
            IReadWriteLocator locator = new Locator();

            locator.Add("Foo", o);
            locator.Remove("Foo");

            Assert.Null(locator.Get("Foo"));
        }

        [Fact]
        public void TripleNestedLocators()
        {
            object o = new object();
            IReadWriteLocator rootLocator = new Locator();
            IReadWriteLocator childLocator = new Locator(rootLocator);
            IReadWriteLocator grandchildLocator = new Locator(childLocator);

            rootLocator.Add("bar", o);

            object result = grandchildLocator.Get("bar");

            Assert.NotNull(result);
            Assert.Same(o, result);
        }
    }
}