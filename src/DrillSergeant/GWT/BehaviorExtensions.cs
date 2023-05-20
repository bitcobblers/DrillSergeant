using System;
using System.Threading.Tasks;

namespace DrillSergeant.GWT;

public static class BehaviorExtensions
{
    public static Behavior<TContext, TInput> Given<TContext,TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, TContext> step)
        where TContext: class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, Task<TContext>> step)
        where TContext: class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext,TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, TContext> step)
        where TContext: class, new()
    {
        behavior.AddStep(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, Task<TContext>> step)
        where TContext: class, new()
    {
        behavior.AddStep(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, IStep step)
        where TContext : class, new()
    {
        behavior.AddStep(step);
        return behavior;
    }

    // ---

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, TContext> step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, Task<TContext>> step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, TContext> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, Task<TContext>> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, IStep step)
        where TContext : class, new()
    {
        behavior.AddStep(step);
        return behavior;
    }

    // ---

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, TContext> step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, Task<TContext>> step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, TContext> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, Task<TContext>> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, IStep step)
        where TContext : class, new()
    {
        behavior.AddStep(step);
        return behavior;
    }
}
