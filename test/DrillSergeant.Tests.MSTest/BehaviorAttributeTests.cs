using System.Xml;
using DrillSergeant.MSTest;
using Shouldly;

namespace DrillSergeant.Tests.MSTest;

public class BehaviorAttributeTests
{
    [TestClass]
    public class CreateTestClassInstanceMethod : BehaviorAttributeTests
    {
        [TestMethod]
        public void TypeWithEmptyConstructorCreatedSuccessfully()
        {
            // Act.
            var result = BehaviorAttribute.CreateTestClassInstance(typeof(StubWithEmptyCtor));

            // Assert.
            result.ShouldBeOfType<StubWithEmptyCtor>();
        }

        [TestMethod]
        public void NullTypeThrowsTestFailedException()
        {
            // Assert.
            Assert.ThrowsException<TestFailedException>(() => 
                BehaviorAttribute.CreateTestClassInstance(null));
        }

        [TestMethod]
        public void TypeWithNonEmptyCtorThrowsTestFailedException()
        {
            // Assert.
            Assert.ThrowsException<TestFailedException>(() =>
                BehaviorAttribute.CreateTestClassInstance(typeof(StubWithNonEmptyCtor)));
        }

        private class StubWithEmptyCtor
        {
        }

        private class StubWithNonEmptyCtor
        {
            public StubWithNonEmptyCtor(int ignored)
            {
            }
        }
    }
}