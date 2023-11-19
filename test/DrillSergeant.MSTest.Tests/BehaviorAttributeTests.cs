namespace DrillSergeant.MSTest.Tests;

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
            // ReSharper disable once UnusedParameter.Local
            public StubWithNonEmptyCtor(int ignored)
            {
            }
        }
    }

    [TestClass]
    public class ExecuteInternalMethod : BehaviorAttributeTests
    {
        [TestMethod]
        public void BehaviorWithNoErrorsPasses()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("EmptyBehavior");
            var arguments = Array.Empty<object?>();
            var token = CancellationToken.None;
            var executor = new BehaviorExecutor(A.Fake<ITestReporter>());

            // Act.
            var result = BehaviorAttribute.ExecuteInternal(executor, instance, method!, arguments, 0, token);

            // Assert.
            result.Outcome.ShouldBe(UnitTestOutcome.Passed);
        }

        [TestMethod]
        public void BehaviorThatThrowsExceptionFails()
        {
            // Arrange.
            var instance = new StubWithBehaviors();
            var method = typeof(StubWithBehaviors).GetMethod("BehaviorThatThrowsException");
            var arguments = Array.Empty<object?>();
            var token = CancellationToken.None;
            var executor = new BehaviorExecutor(A.Fake<ITestReporter>());

            // Act.
            var result = BehaviorAttribute.ExecuteInternal(executor, instance, method!, arguments, 0, token);

            // Assert.
            result.Outcome.ShouldBe(UnitTestOutcome.Failed);
        }

        private class StubWithBehaviors
        {
            [Behavior]
            public void EmptyBehavior()
            {
            }

            [Behavior]
            public void BehaviorThatThrowsException()
            {
                BehaviorBuilder.Current
                    .AddStep(new LambdaStep().Handle(() => throw new Exception("ERROR")));
            }
        }
    }
}