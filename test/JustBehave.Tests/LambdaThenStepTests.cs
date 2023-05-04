using Moq;
using System;
using Xunit;

namespace JustBehave.Tests;

public class LambdaThenStepTests
{
    public record Context();
    public record Input();

    public class TestLambdaThenStep : LambdaThenStep<Context, Input>
    {
    }

    [Fact]
    public void DefaultNameIsNotNull()
    {
        // Arrange.
        var step = new TestLambdaThenStep();

        // Assert.
        Assert.NotNull(step.Name);
    }

    [Fact]
    public void SpecifyingNameSetsNameProperty()
    {
        // Arrange.
        var step = new TestLambdaThenStep();

        // Act.
        step.Named("expected");

        // Assert.
        Assert.Equal("expected", step.Name);
    }

    [Fact]
    public void ActCallsHandler()
    {
        // Arrange.
        var assert = new Mock<TestLambdaThenStep.ThenMethod>();
        var step = new TestLambdaThenStep();

        assert.Setup(x => x(It.IsAny<Context>(), It.IsAny<Input>())).Verifiable();
        step.Handle(assert.Object);

        // Act.
        step.Then(new Context(), new Input());

        // Assert.
        assert.VerifyAll();
    }
}