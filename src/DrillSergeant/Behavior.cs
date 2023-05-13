using DrillSergeant.GWT;
using System;
using System.Collections.Generic;

namespace DrillSergeant;

public abstract class Behavior
{
    protected readonly List<IStep> steps = new();

    public IEnumerable<IStep> Steps => this.steps;
    public Func<object> InitContext { get; protected set; } = () => throw new InvalidOperationException("A context init must be declared first.");
    public Func<object> MapInput { get; protected set; } = () => throw new InvalidOperationException("The input must be mapped.");
    public abstract Type InputType { get; }
}

public class Behavior<TContext, TInput> : Behavior
{
    public override Type InputType => typeof(TInput);

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

    // ---

    
}