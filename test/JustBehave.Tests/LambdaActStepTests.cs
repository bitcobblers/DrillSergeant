using Moq;
using System;
using Xunit;

namespace JustBehave.Tests
{
    using TestLambdaActStep = LambdaActStep<int, int, int>;

    public class LambdaActStepTests
    {
        [Fact]
        public void DefaultNameIsNotNull()
        {
            // Arrange.
            var step = new TestLambdaActStep();

            // Assert.
            Assert.NotNull(step.Name);
        }

        [Fact]
        public void SpecifyingNameSetsNameProperty()
        {
            // Arrange.
            var step = new TestLambdaActStep();

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
            var step = new TestLambdaActStep();

            teardown.Setup(x => x()).Verifiable();
            step.Teardown(teardown.Object);

            // Act.
            step.Dispose();

            // Assert.
            teardown.VerifyAll();
        }

        [Fact]
        public void ActCallsHandler()
        {
            // Arrange.
            var step = new TestLambdaActStep();

            step.Handle((_, _) => 1);

            // Act.
            var result = step.Act(0, 0);

            // Assert.
            Assert.Equal(1, result);
        }

        [Fact]
        public void SettingHandlerToNullReturnsDefaultOnAct()
        {
            // Arrange.
            var step = new TestLambdaActStep();

            step.Handle(null!);

            // Act.
            var result = step.Act(0, 0);

            // Assert.
            Assert.Equal(0, result);
        }
    }
}