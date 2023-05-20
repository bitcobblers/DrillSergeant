using System;
using System.Threading.Tasks;

namespace DrillSergeant.GWT;

public static class BehaviorExtensions
{
    #region Given

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, Delegate step)
        where TContext : class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action step)
        where TContext : class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action<TContext> step)
        where TContext : class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action<TContext, TInput> step)
        where TContext : class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<Task> step)
        where TContext : class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, Task> step)
        where TContext : class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, Task> step)
        where TContext : class, new() =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Delegate step)
    where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action<TContext> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action<TContext, TInput> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<Task> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, Task> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaGivenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Given<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, Task> step)
        where TContext : class, new()
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

    #endregion

    #region When

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Delegate step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action<TContext> step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action<TContext, TInput> step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<Task> step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, Task> step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, Task> step)
        where TContext : class, new() =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Delegate step)
    where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action<TContext> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action<TContext, TInput> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<Task> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, Task> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaWhenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> When<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, Task> step)
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

    #endregion

    #region Then

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Delegate step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action<TContext> step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Action<TContext, TInput> step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<Task> step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, Task> step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, Func<TContext, TInput, Task> step)
        where TContext : class, new() =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Delegate step)
    where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action<TContext> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Action<TContext, TInput> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<Task> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, Task> step)
        where TContext : class, new()
    {
        behavior.AddStep(
            new LambdaThenStep<TContext, TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TContext, TInput> Then<TContext, TInput>(this Behavior<TContext, TInput> behavior, string name, Func<TContext, TInput, Task> step)
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

    #endregion
}
