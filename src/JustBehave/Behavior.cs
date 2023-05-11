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

    // ---

    public Behavior<TContext, TInput> Given(Func<TContext, TInput, TContext> step) =>
        this.Given(step.Method.Name, step);

    public Behavior<TContext, TInput> Given(string name, Func<TContext, TInput, TContext> step)
    {
        this.steps.Add(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return this;
    }

    public Behavior<TContext, TInput> Given(IStep step)
    {
        this.steps.Add(step);
        return this;
    }

    public Behavior<TContext, TInput> Given<TStep>() where TStep : IStep, new()
    {
        this.steps.Add(new TStep());
        return this;
    }

    // ---

    public Behavior<TContext, TInput> When(Func<TContext, TInput, TContext> step) =>
    this.When(step.Method.Name, step);

    public Behavior<TContext, TInput> When(string name, Func<TContext, TInput, TContext> step)
    {
        this.steps.Add(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return this;
    }

    public Behavior<TContext, TInput> When(IStep step)
    {
        this.steps.Add(step);
        return this;
    }

    public Behavior<TContext, TInput> When<TStep>() where TStep : IStep, new()
    {
        this.steps.Add(new TStep());
        return this;
    }

    // ---

    public Behavior<TContext, TInput> Then(Func<TContext, TInput, TContext> step) =>
    this.Then(step.Method.Name, step);

    public Behavior<TContext, TInput> Then(string name, Func<TContext, TInput, TContext> step)
    {
        this.steps.Add(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return this;
    }

    public Behavior<TContext, TInput> Then(IStep step)
    {
        this.steps.Add(step);
        return this;
    }

    public Behavior<TContext, TInput> Then<TStep>() where TStep : IStep, new()
    {
        this.steps.Add(new TStep());
        return this;
    }
}