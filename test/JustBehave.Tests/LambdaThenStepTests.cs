using Moq;
using System;
using Xunit;

namespace JustBehave.Tests
{
    using TestLambdaAssertStep = LambdaThenStep<int, int, int>;

    public class LambdaThenStepTests
    {
        [Fact]
        public void DefaultNameIsNotNull()
        {
            // Arrange.
            var step = new TestLambdaAssertStep();

            // Assert.
            Assert.NotNull(step.Name);
        }

        [Fact]
        public void SpecifyingNameSetsNameProperty()
        {
            // Arrange.
            var step = new TestLambdaAssertStep();

            // Act.
            step.Named("expected");

            // Assert.
            Assert.Equal("expected", step.Name);
        }

        [Fact]
        public void CallsTeardownHandlerOnDispose()
        {
            // Arrage.
            var teardown = new Mock<Action>();
            var step = new TestLambdaAssertStep();

            teardown.Setup(x => x()).Verifiable();
            step.Teardown(teardown.Object);

            // Act.
            step.Dispose();

            // Assert.
            teardown.VerifyAll();
        }

        [Fact]
        public void AssertCallsHandler()
        {
            // Arrange.
            var assert = new Mock<TestLambdaAssertStep.ThenMethod>();
            var step = new TestLambdaAssertStep();

            assert.Setup(x => x(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Verifiable();
            step.Handle(assert.Object);

            // Act.
            step.Then(0, 0, 0);

            // Assert.
            assert.VerifyAll();
        }
    }
}