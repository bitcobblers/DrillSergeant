using Moq;
using System;
using Xunit;

namespace JustBehave.Tests;

public class LambdaGivenStepTests
{
    public record Context(int Value);
    public record Input();

    public class Properties : LambdaGivenStepTests
    {
        [Fact]
        public void DefaultNameIsNotNull()
        {
            // Arrange.
            var step = new LambdaGivenStep<Context, Input>();

            // Assert.
            Assert.NotNull(step.Name);
        }

        [Fact]
        public void SpecifyingNameSetsNameProperty()
        {
            // Arrange.
            var step = new LambdaGivenStep<Context, Input>();

            // Act.
            step.Named("expected");

            // Assert.
            Assert.Equal("expected", step.Name);
        }
    }

    public class TeardownMethod : LambdaGivenStepTests
    {
        [Fact]
        public void CallsTeardownHandlerOnDispose()
        {
            // Arrage.
            var teardown = new Mock<Action>();
            var step = new LambdaGivenStep<Context, Input>();

            teardown.Setup(x => x()).Verifiable();
            step.Teardown(teardown.Object);

            // Act.
            step.Dispose();

            // Assert.
            teardown.VerifyAll();
        }
    }

    public class GivenMethod : LambdaGivenStepTests
    {
        [Fact]
        public void ArrangeCallsHandlerWithReturn()
        {
            // Arrange.
            var step = new LambdaGivenStep<Context, Input>();
            var given = new Mock<Func<Context>>();
            var resolver = new Mock<IDependencyResolver>();

            given.Setup(x => x()).Returns(new Context(Value: 1));
            step.Handle(given.Object);

            // Act.
            var result = step.Execute(resolver.Object);

            // Assert.
            given.VerifyAll();
        }

        [Fact]
        public void ArrangeCallsHandlerWithNoReturn()
        {
            // Arrange.
            var step = new LambdaGivenStep<Context, Input>();
            var given = new Mock<Action>();
            var resolver = new Mock<IDependencyResolver>();

            given.Setup(x => x()).Verifiable();
            step.Handle(given.Object);

            // Act.
            var result = step.Execute(resolver.Object);

            // Assert.
            given.VerifyAll();
        }

        [Fact]
        public void ArrangeCallsLastConfiguredHandler_NoReturn()
        {
            // Arrange.
            var step = new LambdaGivenStep<Context, Input>();
            var givenWithReturn = new Mock<Func<Context>>();
            var givenNoReturn = new Mock<Action>();
            var resolver = new Mock<IDependencyResolver>();

            step.Handle(givenWithReturn.Object);
            step.Handle(givenNoReturn.Object);

            // Act.
            var result = step.Execute(resolver.Object);

            // Assert.
            givenWithReturn.Verify(x => x(), Times.Never());
            givenNoReturn.Verify(x => x(), Times.Once());
        }

        [Fact]
        public void ArrangeCallsLastConfiguredHandler_WithReturn()
        {
            // Arrange.
            var step = new LambdaGivenStep<Context, Input>();
            var givenNoReturn = new Mock<Action>();
            var givenWithReturn = new Mock<Func<Context>>();
            var resolver = new Mock<IDependencyResolver>();

            step.Handle(givenNoReturn.Object);
            step.Handle(givenWithReturn.Object);

            // Act.
            var result = step.Execute(resolver.Object);

            // Assert.
            givenNoReturn.Verify(x => x(), Times.Never());
            givenWithReturn.Verify(x => x(), Times.Once());
        }
    }

    public class ExecuteMethod : LambdaGivenStepTests
    {
        [Fact]
        public void ExecuteCallsConfiguredHandler()
        {
            // Arrange.
            var resolver = new Mock<IDependencyResolver>();
            var step = new LambdaGivenStep<Context, Input>();
            var handler = new Mock<Action>();

            step.Handle(handler.Object);

            // Act.
            step.Execute(resolver.Object);

            // Assert.
            handler.Verify(x => x(), Times.Once());
        }

        [Fact]
        public void ExecuteCallsConfiguredHandlerWithDependency()
        {
            // Arrange.
            var resolver = new Mock<IDependencyResolver>();
            var step = new LambdaGivenStep<Context, Input>();
            var handler = new Mock<Action<Context, Input, IStubInjectable>>();

            step.Handle(handler.Object);

            // Act.
            step.Execute(resolver.Object);

            // Assert.
            handler.Verify(x => x(It.IsAny<Context>(), It.IsAny<Input>(), It.IsAny<IStubInjectable>()), Times.Once());
        }

        public interface IStubInjectable { }
    }
}