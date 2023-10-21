using JetBrains.Annotations;

namespace DrillSergeant;

/// <summary>
/// Defines a step builder that assists in adding named steps to the current behavior.
/// </summary>
public static class StepBuilder
{
    // --- Methods without return value.

    [PublicAPI]
    public static void AddStep(string verb, string name, Action handler)
    {
        AddStep(
            verb,
            new LambdaStep()
                .SetName(name)
                .Handle(handler));
    }

    [PublicAPI]
    public static void AddStepAsync(string verb, string name, Func<Task> handler)
    {
        AddStep(
            verb,
            new LambdaStep()
                .SetName(name)
                .HandleAsync(handler));
    }

    // --- Methods with return value.

    [PublicAPI]
    public static StepResult<T> AddStep<T>(string verb, string name, Func<T> handler)
    {
        var result = new StepResult<T>(name);
        var step = new LambdaStep<T>()
            .SetName(name)
            .SetResult(result)
            .Handle(handler);

        AddStep(verb, step);
        return result;
    }

    [PublicAPI]
    public static AsyncStepResult<T> AddStepAsync<T>(string verb, string name, Func<Task<T>> handler)
    {
        var result = new AsyncStepResult<T>(name);
        var step = new LambdaStep<T>()
            .SetName(name)
            .SetResultAsync(result)
            .HandleAsync(handler);

        AddStep(verb, step);
        return result;
    }

    // --- Fixtures.

    [PublicAPI]
    public static StepResult<T> AddStep<T>(string verb, StepFixture<T> fixture) =>
        AddStep(verb, fixture.Name, fixture.Execute);

    [PublicAPI]
    public static AsyncStepResult<T> AddStepAsync<T>(string verb, AsyncStepFixture<T> fixture) =>
        AddStepAsync(verb, fixture.Name, fixture.Execute);

    // --- Step instances.

    [PublicAPI]
    public static void AddStep<T>() where T : IStep, new() =>
        AddStep(new T());

    [PublicAPI]
    public static void AddStep(IStep step) =>
        AddStep(step.Verb, step);

    [PublicAPI]
    public static void AddStep(string verb, IStep step)
    {
        if (step is LambdaStep lambda)
        {
            lambda.SetVerb(verb);
        }

        BehaviorBuilder.Current.AddStep(step);
    }
}