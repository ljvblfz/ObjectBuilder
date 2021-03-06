using System;
using System.Collections.Generic;
using Xunit;

namespace ObjectBuilder
{
    public class WeakRefDictionaryTest
    {
        [Fact]
        public void AddingToSameKeyTwiceAlwaysThrows()
        {
            object o = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
            dict.Add("foo1", o);

            Assert.Throws<ArgumentException>(
                delegate
                {
                    dict.Add("foo1", o);
                });
        }

        [Fact]
        public void AskingForAKeyThatDoesntExistThrows()
        {
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            Assert.Throws<KeyNotFoundException>(
                delegate
                {
                    object unused = dict["foo"];
                });
        }

        [Fact]
        public void CanAddItemAfterPreviousItemIsCollected()
        {
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
            dict.Add("foo", new object());

            GC.Collect();

            dict.Add("foo", new object());
        }

        [Fact]
        public void CanAddSameObjectTwiceWithDifferentKeys()
        {
            object o = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo1", o);
            dict.Add("foo2", o);

            Assert.Same(dict["foo1"], dict["foo2"]);
        }

        [Fact]
        public void CanEnumerate()
        {
            object o1 = new object();
            object o2 = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo1", o1);
            dict.Add("foo2", o2);

            foreach (KeyValuePair<object, object> kvp in dict)
            {
                Assert.NotNull(kvp);
                Assert.NotNull(kvp.Key);
                Assert.NotNull(kvp.Value);
            }
        }

        [Fact]
        public void CanFindOutIfContainsAKey()
        {
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo", null);
            Assert.True(dict.ContainsKey("foo"));
            Assert.False(dict.ContainsKey("foo2"));
        }

        [Fact]
        public void CanRegisterObjectAndFindItByID()
        {
            object o = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo", o);
            Assert.NotNull(dict["foo"]);
            Assert.Same(o, dict["foo"]);
        }

        [Fact]
        public void CanRegisterTwoObjectsAndGetThemBoth()
        {
            object o1 = new object();
            object o2 = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo1", o1);
            dict.Add("foo2", o2);

            Assert.Same(o1, dict["foo1"]);
            Assert.Same(o2, dict["foo2"]);
        }

        [Fact]
        public void CanRemoveAnObjectThatWasAlreadyAdded()
        {
            object o = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo", o);
            dict.Remove("foo");

            Assert.Throws<KeyNotFoundException>(
                delegate
                {
                    object unused = dict["foo"];
                });
        }

        [Fact]
        public void CountReturnsNumberOfKeysWithLiveValues()
        {
            object o = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo1", o);
            dict.Add("foo2", o);

            Assert.Equal(2, dict.Count);

            o = null;
            GC.Collect();

            Assert.Equal(0, dict.Count);
        }

        [Fact]
        public void KeyCanBeOfArbitraryType()
        {
            object oKey = new object();
            object oVal = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add(oKey, oVal);

            Assert.Same(oVal, dict[oKey]);
        }

        [Fact]
        public void NullIsAValidValue()
        {
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
            dict.Add("foo", null);
            Assert.Null(dict["foo"]);
        }

        [Fact]
        public void RegistrationDoesNotPreventGarbageCollection()
        {
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
            dict.Add("foo", new object());
            GC.Collect();

            Assert.Throws<KeyNotFoundException>(
                delegate
                {
                    object unused = dict["foo"];
                });
        }

        [Fact]
        public void RemovingAKeyDoesNotAffectOtherKeys()
        {
            object o1 = new object();
            object o2 = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo1", o1);
            dict.Add("foo2", o2);
            dict.Remove("foo1");

            Assert.Same(o2, dict["foo2"]);
        }

        [Fact]
        public void RemovingAKeyOfOneObjectDoesNotAffectOtherKeysForSameObject()
        {
            object o = new object();
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();

            dict.Add("foo1", o);
            dict.Add("foo2", o);
            dict.Remove("foo1");

            Assert.Same(o, dict["foo2"]);
        }

        [Fact]
        public void RemovingANonExistantKeyDoesntThrow()
        {
            WeakRefDictionary<object, object> dict = new WeakRefDictionary<object, object>();
            dict.Remove("foo1");
        }
    }
}