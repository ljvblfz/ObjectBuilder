using System;
using System.Collections.Generic;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace ObjectBuilder
{
    [TestFixture]
    public class ReadOnlyLocatorTest
    {
        [Test]
        public void NullInnerLocatorThrows()
        {
            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    new ReadOnlyLocator(null);
                });
        }

        [Test]
        public void CannotCastAReadOnlyLocatorToAReadWriteLocator()
        {
            Locator innerLocator = new Locator();
            ReadOnlyLocator locator = new ReadOnlyLocator(innerLocator);

            Assert.True(locator.ReadOnly);
            Assert.Null(locator as IReadWriteLocator);
        }

        [Test]
        public void ReadOnlyLocatorCountReflectsInnerLocatorCount()
        {
            Locator innerLocator = new Locator();
            ReadOnlyLocator locator = new ReadOnlyLocator(innerLocator);

            innerLocator.Add(1, 1);
            innerLocator.Add(2, 2);

            Assert.Equal(innerLocator.Count, locator.Count);
        }

        [Test]
        public void ParentLocatorOfReadOnlyLocatorIsAlsoReadOnly()
        {
            Locator parentInnerLocator = new Locator();
            Locator childInnerLocator = new Locator(parentInnerLocator);
            ReadOnlyLocator childLocator = new ReadOnlyLocator(childInnerLocator);

            IReadableLocator parentLocator = childLocator.ParentLocator;

            Assert.True(parentLocator.ReadOnly);
            Assert.Null(parentLocator as IReadWriteLocator);
        }

        [Test]
        public void NullParentLocatorOfInnerLocatorReturnsNullParentLocator()
        {
            Locator locator = new Locator();
            ReadOnlyLocator readOnlyLocator = new ReadOnlyLocator(locator);

            IReadableLocator parentLocator = readOnlyLocator.ParentLocator;

            Assert.Null(parentLocator);
        }

        [Test]
        public void ItemsContainedInLocatorContainedInReadOnlyLocator()
        {
            Locator innerLocator = new Locator();
            ReadOnlyLocator locator = new ReadOnlyLocator(innerLocator);

            innerLocator.Add(1, 1);
            innerLocator.Add(2, 2);

            Assert.True(locator.Contains(1));
            Assert.True(locator.Contains(2));
            Assert.False(locator.Contains(3));
        }

        [Test]
        public void CanEnumerateItemsInReadOnlyLocator()
        {
            Locator innerLocator = new Locator();
            ReadOnlyLocator locator = new ReadOnlyLocator(innerLocator);

            innerLocator.Add(1, 1);
            innerLocator.Add(2, 2);

            bool sawOne = false;
            bool sawTwo = false;

            foreach (KeyValuePair<object, object> pair in locator)
            {
                if (pair.Key.Equals(1))
                    sawOne = true;
                if (pair.Key.Equals(2))
                    sawTwo = true;
            }

            Assert.True(sawOne);
            Assert.True(sawTwo);
        }

        [Test]
        public void GenericGetEnforcesDataType()
        {
            Locator innerLocator = new Locator();
            ReadOnlyLocator locator = new ReadOnlyLocator(innerLocator);
            innerLocator.Add(1, 2);

            Assert.Throws<InvalidCastException>(
                delegate
                {
                    locator.Get<string>(1);
                });
        }
    }
}