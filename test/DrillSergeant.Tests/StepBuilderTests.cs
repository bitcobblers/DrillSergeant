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

    [Fact]
    public Task AddingStepWithVerbAndThatReturnsResultSetsVerbOnBehavior()
    {
        // Arrange.
        const string verb = "expected";

        // Act.
        var behavior = BehaviorBuilder.Build(_ =>
        {
            StepBuilder.AddStep(verb, "ignored", () => "ignored");
        });

        // Assert.
        behavior.First().Verb.ShouldBe(verb);

        return Task.CompletedTask;
    }

    [Fact]
    public Task AddingAsyncStepWithVerbAndThatReturnsResultSetsVerbOnBehavior()
    {
        // Arrange.
        const string verb = "expected";

        // Act.
        var behavior = BehaviorBuilder.Build(_ =>
        {
            StepBuilder.AddStepAsync(verb, "ignored", () => Task.FromResult("ignored"));
        });

        // Assert.
        behavior.First().Verb.ShouldBe(verb);

        return Task.CompletedTask;
    }

    [Fact]
    public Task AddingStepWithVerbAndStepFixtureSetsVerbOnBehavior()
    {
        // Arrange.
        const string verb = "expected";
        var fixture = A.Fake<StepFixture<object>>(options =>
        {
            options.WithArgumentsForConstructor(new[] { "ignored" });
        });

        // Act.
        var behavior = BehaviorBuilder.Build(_ =>
        {
            StepBuilder.AddStep(verb, fixture);
        });

        // Assert.
        behavior.First().Verb.ShouldBe(verb);

        return Task.CompletedTask;
    }

    [Fact]
    public Task AddingAsyncStepWithVerbAndAsyncStepFixtureSetsVerbOnBehavior()
    {
        // Arrange.
        const string verb = "expected";
        var fixture = A.Fake<AsyncStepFixture<object>>(options =>
        {
            options.WithArgumentsForConstructor(new[] { "ignored" });
        });

        // Act.
        var behavior = BehaviorBuilder.Build(_ =>
        {
            StepBuilder.AddStepAsync(verb, fixture);
        });

        // Assert.
        behavior.First().Verb.ShouldBe(verb);

        return Task.CompletedTask;
    }

    [Fact]
    public Task AddingStepWithVerbAndStepInstanceSetsVerbOnBehavior()
    {
        // Arrange.
        const string verb = "expected";
        var step = new LambdaStep();

        // Act.
        var behavior = BehaviorBuilder.Build(_ =>
        {
            StepBuilder.AddStep(verb, step);
        });

        // Assert.
        behavior.First().Verb.ShouldBe(verb);

        return Task.CompletedTask;
    }
}