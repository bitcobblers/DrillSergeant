using System;
using System.Threading.Tasks;

namespace DrillSergeant;

/// <summary>
/// Defines a step builder that assists in adding named steps to the current behavior.
/// </summary>
public static class StepBuilder
{
    public static void AddStep(string verb, string name, Action step)
    {
        BehaviorBuilder.Current.AddStep(
            new LambdaStep()
                .SetName(name)
                .SetVerb(verb)
                .Handle(step));
    }

    public static void AddStepAsync(string verb, string name, Func<Task> step)
    {
        BehaviorBuilder.Current.AddStep(
            new LambdaStep()
                .SetName(name)
                .SetVerb(verb)
                .HandleAsync(step));
    }

    public static StepResult<T> AddStep<T>(string verb, string name, Func<T> step)
    {
        var result = new StepResult<T>(name);

        BehaviorBuilder.Current.AddStep(
            new LambdaStep<T>()
                .SetName(name)
                .SetVerb(verb)
                .SetResult(result)
                .Handle(step));

        return result;
    }

    public static AsyncStepResult<T> AddStepAsync<T>(string verb, string name, Func<Task<T>> step)
    {
        var result = new AsyncStepResult<T>(name);

        BehaviorBuilder.Current.AddStep(
            new LambdaStep<T>()
                .SetName(name)
                .SetVerb(verb)
                .SetResultAsync(result)
                .HandleAsync(step));

        return result;
    }

    public static void AddStep(string verb, IStep step)
    {
        if (step is LambdaStep lambda)
        {
            lambda.SetVerb(verb);
        }

        BehaviorBuilder.Current.AddStep(step);
    }

    public static StepResult<T> AddStep<T>(string verb, StepFixture<T> fixture) =>
        AddStep(verb, fixture.Name, fixture.Execute);

    public static AsyncStepResult<T> AddStepAsync<T>(string verb, AsyncStepFixture<T> fixture) =>
        AddStepAsync(verb, fixture.Name, fixture.Execute);
}