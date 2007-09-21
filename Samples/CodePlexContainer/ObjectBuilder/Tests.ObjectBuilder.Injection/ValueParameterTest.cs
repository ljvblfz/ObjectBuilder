using Xunit;

namespace ObjectBuilder
{
    public class ValueParameterTest
    {
        [Test]
        public void ValueParameterReturnsStoredTypeAndValue()
        {
            ValueParameter<int> x = new ValueParameter<int>(12);

            Assert.Equal(typeof(int), x.GetParameterType(null));
            Assert.Equal<object>(12, x.GetValue(null));
        }
    }
}