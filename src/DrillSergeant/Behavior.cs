using System;
using System.Collections;
using System.Collections.Generic;

namespace DrillSergeant;

public abstract class Behavior : IEnumerable<IStep>
{
    protected readonly List<IStep> steps = new();

    public Func<object> InitContext { get; protected set; } = () => throw new InvalidOperationException("A context init must be declared first.");
    public Func<object> MapInput { get; protected set; } = () => throw new InvalidOperationException("The input must be mapped.");

    public IEnumerator<IStep> GetEnumerator() => steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class Behavior<TContext, TInput> : Behavior
    where TContext : class, new()
{
    public Behavior(Func<TInput> mapInput, Func<TContext>? initContext = null)
    {
        this.MapInput = () => mapInput()!;
        this.InitContext = () => (initContext?.Invoke() ?? new TContext());
    }

    public Behavior<TContext, TInput> AddStep(IStep step)
    {
        this.steps.Add(step);
        return this;
    }
}
