using System;
using System.Threading.Tasks;

namespace DrillSergeant.GWT;

public static class BehaviorExtensions
{
    #region Given

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, Delegate step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, Action step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, Action<dynamic> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, Action<TInput> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, Action<dynamic, TInput> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, Func<Task> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, Func<TInput, Task> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaGivenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, string name, Action step)
    {
        behavior.AddStep(
            new LambdaGivenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, string name, Action<dynamic> step)
    {
        behavior.AddStep(
            new LambdaGivenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, string name, Action<TInput> step)
    {
        behavior.AddStep(
            new LambdaGivenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, string name, Action<dynamic, TInput> step)
    {
        behavior.AddStep(
            new LambdaGivenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, string name, Func<Task> step)
    {
        behavior.AddStep(
            new LambdaGivenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, string name, Func<dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaGivenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, string name, Func<TInput, Task> step)
    {
        behavior.AddStep(
            new LambdaGivenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Given<TInput>(this Behavior<TInput> behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    #endregion

    #region When

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, Delegate step) =>
      behavior.When(step.Method.Name, step);

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, Action step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, Action<dynamic> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, Action<TInput> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, Action<dynamic, TInput> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, Func<Task> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, Func<TInput, Task> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaWhenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, string name, Action step)
    {
        behavior.AddStep(
            new LambdaWhenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, string name, Action<dynamic> step)
    {
        behavior.AddStep(
            new LambdaWhenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, string name, Action<TInput> step)
    {
        behavior.AddStep(
            new LambdaWhenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, string name, Func<Task> step)
    {
        behavior.AddStep(
            new LambdaWhenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, string name, Func<dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaWhenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, string name, Func<TInput, Task> step)
    {
        behavior.AddStep(
            new LambdaWhenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> When<TInput>(this Behavior<TInput> behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    #endregion

    #region Then

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, Delegate step) =>
       behavior.Then(step.Method.Name, step);

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, Action step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, Action<dynamic> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, Action<TInput> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, Action<dynamic, TInput> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, Func<Task> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, Func<TInput, Task> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaThenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, string name, Action step)
    {
        behavior.AddStep(
            new LambdaThenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, string name, Action<dynamic> step)
    {
        behavior.AddStep(
            new LambdaThenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, string name, Action<TInput> step)
    {
        behavior.AddStep(
            new LambdaThenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, string name, Func<Task> step)
    {
        behavior.AddStep(
            new LambdaThenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, string name, Func<dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaThenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, string name, Func<TInput, Task> step)
    {
        behavior.AddStep(
            new LambdaThenStep<TInput>()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior<TInput> Then<TInput>(this Behavior<TInput> behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    #endregion
}
