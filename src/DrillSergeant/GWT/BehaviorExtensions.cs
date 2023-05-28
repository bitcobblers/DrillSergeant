using System;
using System.Threading.Tasks;

namespace DrillSergeant.GWT;

public static class BehaviorExtensions
{
    #region Given

    public static Behavior Given(this Behavior behavior, Delegate step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior Given(this Behavior behavior, Action step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior Given(this Behavior behavior, Action<dynamic> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior Given(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior GivenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.GivenAsync(step.Method.Name, step);

    public static Behavior GivenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.GivenAsync(step.Method.Name, step);

    public static Behavior GivenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.GivenAsync(step.Method.Name, step);

    public static Behavior Given(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaGivenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior Given(this Behavior behavior, string name, Action step)
    {
        behavior.AddStep(
            new LambdaGivenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior Given(this Behavior behavior, string name, Action<dynamic> step)
    {
        behavior.AddStep(
            new LambdaGivenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior Given(this Behavior behavior, string name, Action<dynamic, dynamic> step)
    {
        behavior.AddStep(
            new LambdaGivenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior GivenAsync(this Behavior behavior, string name, Func<Task> step)
    {
        behavior.AddStep(
            new LambdaGivenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior GivenAsync(this Behavior behavior, string name, Func<dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaGivenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior GivenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaGivenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior Given(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    #endregion

    #region When

    public static Behavior When(this Behavior behavior, Delegate step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior When(this Behavior behavior, Action step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior When(this Behavior behavior, Action<dynamic> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior When(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior WhenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.WhenAsync(step.Method.Name, step);

    public static Behavior WhenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.WhenAsync(step.Method.Name, step);

    public static Behavior WhenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.WhenAsync(step.Method.Name, step);

    public static Behavior When(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaWhenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior When(this Behavior behavior, string name, Action step)
    {
        behavior.AddStep(
            new LambdaWhenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior When(this Behavior behavior, string name, Action<dynamic> step)
    {
        behavior.AddStep(
            new LambdaWhenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior When(this Behavior behavior, string name, Action<dynamic, dynamic> step)
    {
        behavior.AddStep(
            new LambdaWhenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior WhenAsync(this Behavior behavior, string name, Func<Task> step)
    {
        behavior.AddStep(
            new LambdaWhenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior WhenAsync(this Behavior behavior, string name, Func<dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaWhenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior WhenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaWhenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior When(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    #endregion

    #region Then

    public static Behavior Then(this Behavior behavior, Delegate step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior Then(this Behavior behavior, Action step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior Then(this Behavior behavior, Action<dynamic> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior Then(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior ThenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.ThenAsync(step.Method.Name, step);

    public static Behavior ThenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.ThenAsync(step.Method.Name, step);

    public static Behavior ThenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.ThenAsync(step.Method.Name, step);

    public static Behavior Then(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaThenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior Then(this Behavior behavior, string name, Action step)
    {
        behavior.AddStep(
            new LambdaThenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior Then(this Behavior behavior, string name, Action<dynamic> step)
    {
        behavior.AddStep(
            new LambdaThenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior Then(this Behavior behavior, string name, Action<dynamic, dynamic> step)
    {
        behavior.AddStep(
            new LambdaThenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior ThenAsync(this Behavior behavior, string name, Func<Task> step)
    {
        behavior.AddStep(
            new LambdaThenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior ThenAsync(this Behavior behavior, string name, Func<dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaThenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior ThenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaThenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior Then(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    #endregion
}
