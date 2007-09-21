using Xunit;

namespace ObjectBuilder
{
    public class DependencyAttributeTest
    {
        [Test]
        public void DefaultBuildKeyIsAnnotatedMemberType()
        {
            DependencyAttribute attribute = new DependencyAttribute();

            IParameter result = attribute.CreateParameter(typeof(object));

            DependencyParameter parameter = Assert.IsType<DependencyParameter>(result);
            Assert.Equal<object>(typeof(object), parameter.BuildKey);
        }

        [Test]
        public void DefaultNotPresentBehaviorIsBuild()
        {
            DependencyAttribute attribute = new DependencyAttribute("Foo");

            IParameter result = attribute.CreateParameter(typeof(object));

            DependencyParameter parameter = Assert.IsType<DependencyParameter>(result);
            Assert.Equal(NotPresentBehavior.Build, parameter.NotPresentBehavior);
        }

        [Test]
        public void ReturnsDependencyParameter()
        {
            DependencyAttribute attribute = new DependencyAttribute("Foo");
            attribute.NotPresentBehavior = NotPresentBehavior.Throw;

            IParameter result = attribute.CreateParameter(typeof(object));

            DependencyParameter parameter = Assert.IsType<DependencyParameter>(result);
            Assert.Equal<object>("Foo", parameter.BuildKey);
            Assert.Equal(NotPresentBehavior.Throw, parameter.NotPresentBehavior);
        }
    }
}