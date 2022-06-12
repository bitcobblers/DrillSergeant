using Moq;
using System;
using Xunit;

namespace JustBehave.Tests
{
    using TestLambdaArrangeStep = LambdaGivenStep<int, int>;

    public class LambdaGivenStepTests
    {
        [Fact]
        public void DefaultNameIsNotNull()
        {
            // Arrange.
            var step = new TestLambdaArrangeStep();

            // Assert.
            Assert.NotNull(step.Name);
        }

        [Fact]
        public void SpecifyingNameSetsNameProperty()
        {
            // Arrange.
            var step = new TestLambdaArrangeStep();

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
            var step = new TestLambdaArrangeStep();

            teardown.Setup(x => x()).Verifiable();
            step.Teardown(teardown.Object);

            // Act.
            step.Dispose();

            // Assert.
            teardown.VerifyAll();
        }

        [Fact]
        public void ArrangeCallsHandlerWithReturn()
        {
            // Arrange.
            var step = new TestLambdaArrangeStep();
            var arrange = new Mock<TestLambdaArrangeStep.GivenWithReturnMethod>();

            arrange.Setup(x => x(It.IsAny<int>(), It.IsAny<int>())).Verifiable();
            step.Handle(arrange.Object);

            // Act.
            var result = step.Given(0, 0);

            // Assert.
            arrange.VerifyAll();
        }

        [Fact]
        public void ArrangeCallsHandlerWithNoReturn()
        {
            // Arrange.
            var step = new TestLambdaArrangeStep();
            var arrange = new Mock<TestLambdaArrangeStep.GivenNoReturnMethod>();

            arrange.Setup(x => x(It.IsAny<int>(), It.IsAny<int>())).Verifiable();
            step.Handle(arrange.Object);

            // Act.
            var result = step.Given(0, 0);

            // Assert.
            arrange.VerifyAll();
        }

        [Fact]
        public void ArrangeCallsLastConfiguredHandler_NoReturn()
        {
            // Arrange.
            var step = new TestLambdaArrangeStep();
            var arrangeWithReturn = new Mock<TestLambdaArrangeStep.GivenWithReturnMethod>();
            var arrangeNoReturn = new Mock<TestLambdaArrangeStep.GivenNoReturnMethod>();

            step.Handle(arrangeWithReturn.Object);
            step.Handle(arrangeNoReturn.Object);

            // Act.
            var result = step.Given(0, 0);

            // Assert.
            arrangeWithReturn.Verify(x => x(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            arrangeNoReturn.Verify(x => x(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void ArrangeCallsLastConfiguredHandler_WithReturn()
        {
            // Arrange.
            var step = new TestLambdaArrangeStep();
            var arrangeNoReturn = new Mock<TestLambdaArrangeStep.GivenNoReturnMethod>();
            var arrangeWithReturn = new Mock<TestLambdaArrangeStep.GivenWithReturnMethod>();

            step.Handle(arrangeNoReturn.Object);
            step.Handle(arrangeWithReturn.Object);

            // Act.
            var result = step.Given(0, 0);

            // Assert.
            arrangeNoReturn.Verify(x => x(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            arrangeWithReturn.Verify(x => x(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }
    }
}