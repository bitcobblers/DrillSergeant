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
{
    public Behavior<TContext, TInput> WithContext(Func<TContext> initContext)
    {
        this.InitContext = () => initContext()!;
        return this;
    }

    public Behavior<TContext, TInput> WithInput(Func<TInput> mapInput)
    {
        this.MapInput = () => mapInput()!;
        return this;
    }

    public Behavior<TContext, TInput> AddStep(IStep step)
    {
        this.steps.Add(step);
        return this;
    }
}
