using Xunit;

namespace ObjectBuilder
{
    public class DependencyAttributeTest
    {
        [Fact]
        public void DefaultBuildKeyIsAnnotatedMemberType()
        {
            DependencyAttribute attribute = new DependencyAttribute();

            IParameter result = attribute.CreateParameter(typeof(object));

            DependencyParameter parameter = Assert.IsType<DependencyParameter>(result);
            Assert.Equal<object>(typeof(object), parameter.BuildKey);
        }

        [Fact]
        public void DefaultNotPresentBehaviorIsBuild()
        {
            DependencyAttribute attribute = new DependencyAttribute("Foo");

            IParameter result = attribute.CreateParameter(typeof(object));

            DependencyParameter parameter = Assert.IsType<DependencyParameter>(result);
            Assert.Equal(NotPresentBehavior.Build, parameter.NotPresentBehavior);
        }

        [Fact]
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