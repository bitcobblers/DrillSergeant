using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace DrillSergeant;

public class Behavior<TInput> : IBehavior
{
    protected readonly List<IStep> steps = new();

    public Behavior(TInput input, object? context = null)
    {
        this.Input = input!;
        this.Context = context ?? new ExpandoObject();
    }

    public object Context { get; }

    public object Input { get; }

    public bool LogContext { get; private set; }

    public Behavior<TInput> AddStep(IStep step)
    {
        this.steps.Add(step);
        return this;
    }

    public Behavior<TInput> EnableContextLogging()
    {
        this.LogContext = true;
        return this;
    }

    public IEnumerator<IStep> GetEnumerator() => steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
