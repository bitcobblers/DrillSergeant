using System;
using System.Runtime.CompilerServices;
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

    public static Behavior Given<TStep>(this Behavior behavior) 
        where TStep : IStep, new()
    {
        behavior.AddStep(new TStep());
        return behavior;
    }

    #endregion

    #region And

    public static Behavior And(this Behavior behavior, Delegate step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior And(this Behavior behavior, Action step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior And(this Behavior behavior, Action<dynamic> step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior And(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior AndAsync(this Behavior behavior, Func<Task> step) =>
        behavior.AndAsync(step.Method.Name, step);

    public static Behavior AndAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.AndAsync(step.Method.Name, step);

    public static Behavior AndAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.AndAsync(step.Method.Name, step);

    public static Behavior And(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaAndStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior And(this Behavior behavior, string name, Action step)
    {
        behavior.AddStep(
            new LambdaAndStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior And(this Behavior behavior, string name, Action<dynamic> step)
    {
        behavior.AddStep(
            new LambdaAndStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior And(this Behavior behavior, string name, Action<dynamic, dynamic> step)
    {
        behavior.AddStep(
            new LambdaAndStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior AndAsync(this Behavior behavior, string name, Func<Task> step)
    {
        behavior.AddStep(
            new LambdaAndStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior AndAsync(this Behavior behavior, string name, Func<dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaAndStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior AndAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaAndStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior And(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    public static Behavior And<TStep>(this Behavior behavior)
        where TStep : IStep, new()
    {
        behavior.AddStep(new TStep());
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

    public static Behavior When<TStep>(this Behavior behavior)
        where TStep : IStep, new()
    {
        behavior.AddStep(new TStep());
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

    public static Behavior Then<TStep>(this Behavior behavior)
        where TStep : IStep, new()
    {
        behavior.AddStep(new TStep());
        return behavior;
    }

    #endregion

    #region But

    public static Behavior But(this Behavior behavior, Delegate step) =>
        behavior.But(step.Method.Name, step);

    public static Behavior But(this Behavior behavior, Action step) =>
        behavior.But(step.Method.Name, step);

    public static Behavior But(this Behavior behavior, Action<dynamic> step) =>
        behavior.But(step.Method.Name, step);

    public static Behavior But(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.But(step.Method.Name, step);

    public static Behavior ButAsync(this Behavior behavior, Func<Task> step) =>
        behavior.ButAsync(step.Method.Name, step);

    public static Behavior ButAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.ButAsync(step.Method.Name, step);

    public static Behavior ButAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.ButAsync(step.Method.Name, step);

    public static Behavior But(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaButStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior But(this Behavior behavior, string name, Action step)
    {
        behavior.AddStep(
            new LambdaButStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior But(this Behavior behavior, string name, Action<dynamic> step)
    {
        behavior.AddStep(
            new LambdaButStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior But(this Behavior behavior, string name, Action<dynamic, dynamic> step)
    {
        behavior.AddStep(
            new LambdaButStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior ButAsync(this Behavior behavior, string name, Func<Task> step)
    {
        behavior.AddStep(
            new LambdaButStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior ButAsync(this Behavior behavior, string name, Func<dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaButStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior ButAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step)
    {
        behavior.AddStep(
            new LambdaButStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    public static Behavior But(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    public static Behavior But<TStep>(this Behavior behavior)
        where TStep : IStep, new()
    {
        behavior.AddStep(new TStep());
        return behavior;
    }

    #endregion
}
