using Moq;
using System;
using Xunit;

namespace JustBehave.Tests;

public class LambdaWhenStepTests
{
    public record Context();
    public record Input();

    public class TestLambdaWhenStep : LambdaWhenStep<Context,Input>
    {
    }

    [Fact]
    public void DefaultNameIsNotNull()
    {
        // Arrange.
        var step = new TestLambdaWhenStep();

        // Assert.
        Assert.NotNull(step.Name);
    }

    [Fact]
    public void SpecifyingNameSetsNameProperty()
    {
        // Arrange.
        var step = new TestLambdaWhenStep();

        // Act.
        step.Named("expected");

        // Assert.
        Assert.Equal("expected", step.Name);
    }
}
