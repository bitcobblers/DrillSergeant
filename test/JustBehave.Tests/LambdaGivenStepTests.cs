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
            var result = step.Execute(new Context(0), new Input(), resolver.Object);

            // Assert.
            given.VerifyAll();
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
            var handler = new Mock<Func<Context, Input, Context>>();

            step.Handle(handler.Object);

            // Act.
            step.Execute(new Context(0), new Input(), resolver.Object);

            // Assert.
            handler.Verify(x => x(It.IsAny<Context>(), It.IsAny<Input>()), Times.Once());
        }

        [Fact]
        public void ExecuteCallsConfiguredHandlerWithDependency()
        {
            // Arrange.
            var resolver = new Mock<IDependencyResolver>();
            var step = new LambdaGivenStep<Context, Input>();
            var handler = new Mock<Func<Context, Input, IStubInjectable, Context>>();

            step.Handle(handler.Object);

            // Act.
            step.Execute(new Context(0), new Input(), resolver.Object);

            // Assert.
            handler.Verify(x => x(It.IsAny<Context>(), It.IsAny<Input>(), It.IsAny<IStubInjectable>()), Times.Once());
        }

        public interface IStubInjectable { }
    }
}