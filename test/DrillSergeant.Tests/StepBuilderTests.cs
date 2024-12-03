namespace DrillSergeant.Tests;

public class StepBuilderTests
{
    [Fact]
    public Task AddingStepWithVerbAndActionSetsVerbOnBehavior()
    {
        // Arrange.
        const string verb = "expected";

        // Act.
        var behavior = BehaviorBuilder.Build(_ =>
        {
            StepBuilder.AddStep(verb, "ignored", () => { });
        });

        // Assert.
        behavior.First().Verb.ShouldBe(verb);

        return Task.CompletedTask;
    }

    [Fact]
    public Task AddingAsyncStepWithVerbAndFuncSetsVerbOnBehavior()
    {
        // Arrange.
        const string verb = "expected";

        // Act.
        var behavior = BehaviorBuilder.Build(_ =>
        {
            StepBuilder.AddStepAsync(verb, "ignored", () => Task.CompletedTask);
        });

        // Assert.
        behavior.First().Verb.ShouldBe(verb);

        return Task.CompletedTask;
    }
}