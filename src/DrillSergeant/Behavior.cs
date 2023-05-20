using System;
using System.Collections;
using System.Collections.Generic;

namespace DrillSergeant;

public class Behavior<TContext, TInput> : IBehavior
    where TContext : class, new()
{
    protected readonly List<IStep> steps = new();

    public Behavior(TInput input, TContext? context = null)
    {
        this.Input = input!;
        this.Context = context ?? new TContext();
    }

    public object Context { get; }

    public object Input { get; }

    public bool LogContext { get; private set; }

    public IDependencyResolver Resolver { get; private set; } = new DefaultResolver();

    public Behavior<TContext, TInput> AddStep(IStep step)
    {
        this.steps.Add(step);
        return this;
    }

    public Behavior<TContext, TInput> EnableContextLogging()
    {
        this.LogContext = true;
        return this;
    }

    public Behavior<TContext, TInput> ConfigureResolver(Func<IDependencyResolver?> configureResolver)
    {
        this.Resolver = configureResolver?.Invoke() ?? this.Resolver;
        return this;
    }

    public IEnumerator<IStep> GetEnumerator() => steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
