using System;
using System.Threading.Tasks;

namespace DrillSergeant.GWT;

public static class BehaviorExtensions
{
    #region Given

    public static Behavior Given(this Behavior behavior, Action step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior Given(this Behavior behavior, Action<dynamic> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior Given(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior Given<TContext>(this Behavior behavior, Action<TContext> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior Given<TInput>(this Behavior behavior, Action<dynamic, TInput> step) =>
        behavior.Given(step.Method.Name, step);

    public static Behavior Given<TContext, TInput>(this Behavior behavior, Action<TContext, TInput> step) =>
        behavior.Given(step.Method.Name, step);

    // ---

    public static Behavior GivenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.GivenInternal(step.Method.Name, step);

    public static Behavior GivenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.GivenInternal(step.Method.Name, step);

    public static Behavior GivenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.GivenInternal(step.Method.Name, step);

    public static Behavior GivenAsync<TContext>(this Behavior behavior, Func<TContext, Task> step) =>
        behavior.GivenInternal(step.Method.Name, step);

    public static Behavior GivenAsync<TInput>(this Behavior behavior, Func<dynamic, TInput, Task> step) =>
        behavior.GivenInternal(step.Method.Name, step);

    public static Behavior GivenAsync<TContext, TInput>(this Behavior behavior, Func<TContext, TInput, Task> step) =>
        behavior.GivenInternal(step.Method.Name, step);

    // ---

    public static Behavior Given(this Behavior behavior, string name, Action step) =>
        behavior.GivenInternal(name, step);

    public static Behavior Given(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior Given(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior Given<TContext>(this Behavior behavior, string name, Action<TContext> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior Given<TInput>(this Behavior behavior, string name, Action<dynamic, TInput> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior Given<TContext, TInput>(this Behavior behavior, string name, Action<TContext, TInput> step) =>
        behavior.GivenInternal(name, step);

    // ---

    public static Behavior GivenAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior GivenAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior GivenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior GivenAsync<TContext>(this Behavior behavior, string name, Func<TContext, dynamic, Task> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior GivenAsync<TInput>(this Behavior behavior, string name, Func<dynamic, TInput, Task> step) =>
        behavior.GivenInternal(name, step);

    public static Behavior GivenAsync<TContext, TInput>(this Behavior behavior, string name, Func<TContext, TInput, Task> step) =>
        behavior.GivenInternal(name, step);

    // ---

    public static Behavior Given<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.Given(new TStep());

    public static Behavior Given(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    // ---

    private static Behavior GivenInternal(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaGivenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    #endregion

    #region And

    public static Behavior And(this Behavior behavior, Action step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior And(this Behavior behavior, Action<dynamic> step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior And(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior And<TContext>(this Behavior behavior, Action<TContext> step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior And<TInput>(this Behavior behavior, Action<dynamic, TInput> step) =>
        behavior.And(step.Method.Name, step);

    public static Behavior And<TContext, TInput>(this Behavior behavior, Action<TContext, TInput> step) =>
        behavior.And(step.Method.Name, step);

    // ---

    public static Behavior AndAsync(this Behavior behavior, Func<Task> step) =>
        behavior.AndInternal(step.Method.Name, step);

    public static Behavior AndAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.AndInternal(step.Method.Name, step);

    public static Behavior AndAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.AndInternal(step.Method.Name, step);

    public static Behavior AndAsync<TContext>(this Behavior behavior, Func<TContext, Task> step) =>
        behavior.AndInternal(step.Method.Name, step);

    public static Behavior AndAsync<TInput>(this Behavior behavior, Func<dynamic, TInput, Task> step) =>
        behavior.AndInternal(step.Method.Name, step);

    public static Behavior AndAsync<TContext, TInput>(this Behavior behavior, Func<TContext, TInput, Task> step) =>
        behavior.AndInternal(step.Method.Name, step);

    // ---

    public static Behavior And(this Behavior behavior, string name, Action step) =>
        behavior.AndInternal(name, step);

    public static Behavior And(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.AndInternal(name, step);

    public static Behavior And(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.AndInternal(name, step);

    public static Behavior And<TContext>(this Behavior behavior, string name, Action<TContext> step) =>
        behavior.AndInternal(name, step);

    public static Behavior And<TInput>(this Behavior behavior, string name, Action<dynamic, TInput> step) =>
        behavior.AndInternal(name, step);

    public static Behavior And<TContext, TInput>(this Behavior behavior, string name, Action<TContext, TInput> step) =>
        behavior.AndInternal(name, step);

    // ---

    public static Behavior AndAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.AndInternal(name, step);

    public static Behavior AndAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.AndInternal(name, step);

    public static Behavior AndAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.AndInternal(name, step);

    public static Behavior AndAsync<TContext>(this Behavior behavior, string name, Func<TContext, dynamic, Task> step) =>
        behavior.AndInternal(name, step);

    public static Behavior AndAsync<TInput>(this Behavior behavior, string name, Func<dynamic, TInput, Task> step) =>
        behavior.AndInternal(name, step);

    public static Behavior AndAsync<TContext, TInput>(this Behavior behavior, string name, Func<TContext, TInput, Task> step) =>
        behavior.AndInternal(name, step);

    // ---

    public static Behavior And<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.And(new TStep());

    public static Behavior And(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    // ---

    private static Behavior AndInternal(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaAndStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    #endregion

    #region When

    public static Behavior When(this Behavior behavior, Action step) =>
       behavior.When(step.Method.Name, step);

    public static Behavior When(this Behavior behavior, Action<dynamic> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior When(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior When<TContext>(this Behavior behavior, Action<TContext> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior When<TInput>(this Behavior behavior, Action<dynamic, TInput> step) =>
        behavior.When(step.Method.Name, step);

    public static Behavior When<TContext, TInput>(this Behavior behavior, Action<TContext, TInput> step) =>
        behavior.When(step.Method.Name, step);

    // ---

    public static Behavior WhenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.WhenInternal(step.Method.Name, step);

    public static Behavior WhenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.WhenInternal(step.Method.Name, step);

    public static Behavior WhenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.WhenInternal(step.Method.Name, step);

    public static Behavior WhenAsync<TContext>(this Behavior behavior, Func<TContext, Task> step) =>
        behavior.WhenInternal(step.Method.Name, step);

    public static Behavior WhenAsync<TInput>(this Behavior behavior, Func<dynamic, TInput, Task> step) =>
        behavior.WhenInternal(step.Method.Name, step);

    public static Behavior WhenAsync<TContext, TInput>(this Behavior behavior, Func<TContext, TInput, Task> step) =>
        behavior.WhenInternal(step.Method.Name, step);

    // ---

    public static Behavior When(this Behavior behavior, string name, Action step) =>
        behavior.WhenInternal(name, step);

    public static Behavior When(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior When(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior When<TContext>(this Behavior behavior, string name, Action<TContext> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior When<TInput>(this Behavior behavior, string name, Action<dynamic, TInput> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior When<TContext, TInput>(this Behavior behavior, string name, Action<TContext, TInput> step) =>
        behavior.WhenInternal(name, step);

    // ---

    public static Behavior WhenAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior WhenAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior WhenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior WhenAsync<TContext>(this Behavior behavior, string name, Func<TContext, dynamic, Task> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior WhenAsync<TInput>(this Behavior behavior, string name, Func<dynamic, TInput, Task> step) =>
        behavior.WhenInternal(name, step);

    public static Behavior WhenAsync<TContext, TInput>(this Behavior behavior, string name, Func<TContext, TInput, Task> step) =>
        behavior.WhenInternal(name, step);

    // ---

    public static Behavior When<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.When(new TStep());

    public static Behavior When(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    // ---

    private static Behavior WhenInternal(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaWhenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    #endregion

    #region Then

    public static Behavior Then(this Behavior behavior, Action step) =>
       behavior.Then(step.Method.Name, step);

    public static Behavior Then(this Behavior behavior, Action<dynamic> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior Then(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior Then<TContext>(this Behavior behavior, Action<TContext> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior Then<TInput>(this Behavior behavior, Action<dynamic, TInput> step) =>
        behavior.Then(step.Method.Name, step);

    public static Behavior Then<TContext, TInput>(this Behavior behavior, Action<TContext, TInput> step) =>
        behavior.Then(step.Method.Name, step);

    // ---

    public static Behavior ThenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.ThenInternal(step.Method.Name, step);

    public static Behavior ThenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.ThenInternal(step.Method.Name, step);

    public static Behavior ThenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.ThenInternal(step.Method.Name, step);

    public static Behavior ThenAsync<TContext>(this Behavior behavior, Func<TContext, Task> step) =>
        behavior.ThenInternal(step.Method.Name, step);

    public static Behavior ThenAsync<TInput>(this Behavior behavior, Func<dynamic, TInput, Task> step) =>
        behavior.ThenInternal(step.Method.Name, step);

    public static Behavior ThenAsync<TContext, TInput>(this Behavior behavior, Func<TContext, TInput, Task> step) =>
        behavior.ThenInternal(step.Method.Name, step);

    // ---

    public static Behavior Then(this Behavior behavior, string name, Action step) =>
        behavior.ThenInternal(name, step);

    public static Behavior Then(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior Then(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior Then<TContext>(this Behavior behavior, string name, Action<TContext> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior Then<TInput>(this Behavior behavior, string name, Action<dynamic, TInput> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior Then<TContext, TInput>(this Behavior behavior, string name, Action<TContext, TInput> step) =>
        behavior.ThenInternal(name, step);

    // ---

    public static Behavior ThenAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior ThenAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior ThenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior ThenAsync<TContext>(this Behavior behavior, string name, Func<TContext, dynamic, Task> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior ThenAsync<TInput>(this Behavior behavior, string name, Func<dynamic, TInput, Task> step) =>
        behavior.ThenInternal(name, step);

    public static Behavior ThenAsync<TContext, TInput>(this Behavior behavior, string name, Func<TContext, TInput, Task> step) =>
        behavior.ThenInternal(name, step);

    // ---

    public static Behavior Then<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.Then(new TStep());

    public static Behavior Then(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    // ---

    private static Behavior ThenInternal(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaThenStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    #endregion

    #region But

    public static Behavior But(this Behavior behavior, Action step) =>
      behavior.But(step.Method.Name, step);

    public static Behavior But(this Behavior behavior, Action<dynamic> step) =>
        behavior.But(step.Method.Name, step);

    public static Behavior But(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.But(step.Method.Name, step);

    public static Behavior But<TContext>(this Behavior behavior, Action<TContext> step) =>
        behavior.But(step.Method.Name, step);

    public static Behavior But<TInput>(this Behavior behavior, Action<dynamic, TInput> step) =>
        behavior.But(step.Method.Name, step);

    public static Behavior But<TContext, TInput>(this Behavior behavior, Action<TContext, TInput> step) =>
        behavior.But(step.Method.Name, step);

    // ---

    public static Behavior ButAsync(this Behavior behavior, Func<Task> step) =>
        behavior.ButInternal(step.Method.Name, step);

    public static Behavior ButAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.ButInternal(step.Method.Name, step);

    public static Behavior ButAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.ButInternal(step.Method.Name, step);

    public static Behavior ButAsync<TContext>(this Behavior behavior, Func<TContext, Task> step) =>
        behavior.ButInternal(step.Method.Name, step);

    public static Behavior ButAsync<TInput>(this Behavior behavior, Func<dynamic, TInput, Task> step) =>
        behavior.ButInternal(step.Method.Name, step);

    public static Behavior ButAsync<TContext, TInput>(this Behavior behavior, Func<TContext, TInput, Task> step) =>
        behavior.ButInternal(step.Method.Name, step);

    // ---

    public static Behavior But(this Behavior behavior, string name, Action step) =>
        behavior.ButInternal(name, step);

    public static Behavior But(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.ButInternal(name, step);

    public static Behavior But(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.ButInternal(name, step);

    public static Behavior But<TContext>(this Behavior behavior, string name, Action<TContext> step) =>
        behavior.ButInternal(name, step);

    public static Behavior But<TInput>(this Behavior behavior, string name, Action<dynamic, TInput> step) =>
        behavior.ButInternal(name, step);

    public static Behavior But<TContext, TInput>(this Behavior behavior, string name, Action<TContext, TInput> step) =>
        behavior.ButInternal(name, step);

    // ---

    public static Behavior ButAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.ButInternal(name, step);

    public static Behavior ButAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.ButInternal(name, step);

    public static Behavior ButAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.ButInternal(name, step);

    public static Behavior ButAsync<TContext>(this Behavior behavior, string name, Func<TContext, dynamic, Task> step) =>
        behavior.ButInternal(name, step);

    public static Behavior ButAsync<TInput>(this Behavior behavior, string name, Func<dynamic, TInput, Task> step) =>
        behavior.ButInternal(name, step);

    public static Behavior ButAsync<TContext, TInput>(this Behavior behavior, string name, Func<TContext, TInput, Task> step) =>
        behavior.ButInternal(name, step);

    // ---

    public static Behavior But<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.But(new TStep());

    public static Behavior But(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

    // ---

    private static Behavior ButInternal(this Behavior behavior, string name, Delegate step)
    {
        behavior.AddStep(
            new LambdaButStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }

    #endregion
}
