using System.Collections;
using System.Collections.Generic;

namespace DrillSergeant;

public abstract class Behavior : IEnumerable<IStep>
{
    protected readonly List<IStep> steps = new();

    public object Context { get; }
    public object Input { get; }

    public Behavior(object context, object input)
    {
        this.Context = context;
        this.Input = input;
    }

    public IEnumerator<IStep> GetEnumerator() => steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class Behavior<TContext, TInput> : Behavior
    where TContext : class, new()
{
    public Behavior(TInput input, TContext? context = null)
        : base(input!, context ?? new TContext())
    {
    }

    public Behavior<TContext, TInput> AddStep(IStep step)
    {
        this.steps.Add(step);
        return this;
    }
}
