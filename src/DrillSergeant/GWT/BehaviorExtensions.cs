using System;
using System.Threading.Tasks;

namespace DrillSergeant.GWT;

public static class BehaviorExtensions
{
    #region Given

    /// <summary>
    /// Adds a 'Given' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior Given(this Behavior behavior, Delegate step) =>
        behavior.Given(step.Method.Name, step);

    /// <inheritdoc cref="Given(Behavior, Delegate)" />
    public static Behavior Given(this Behavior behavior, Action step) =>
        behavior.Given(step.Method.Name, step);

    /// <inheritdoc cref="Given(Behavior, Delegate)" />
    public static Behavior Given(this Behavior behavior, Action<dynamic> step) =>
        behavior.Given(step.Method.Name, step);

    /// <inheritdoc cref="Given(Behavior, Delegate)" />
    public static Behavior Given(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.Given(step.Method.Name, step);

    /// <inheritdoc cref="Given(Behavior, Delegate)" />
    public static Behavior GivenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.GivenAsync(step.Method.Name, step);

    /// <inheritdoc cref="Given(Behavior, Delegate)" />
    public static Behavior GivenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.GivenAsync(step.Method.Name, step);

    /// <inheritdoc cref="Given(Behavior, Delegate)" />
    public static Behavior GivenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.GivenAsync(step.Method.Name, step);

    /// <summary>
    /// Adds a 'Given' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="name">The name of the step.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior Given(this Behavior behavior, string name, Delegate step) =>
        behavior.GivenInternal(name, step);

    /// <inheritdoc cref="Given(Behavior, string, Delegate)" />
    public static Behavior Given(this Behavior behavior, string name, Action step) =>
        behavior.GivenInternal(name, step);

    /// <inheritdoc cref="Given(Behavior, string, Delegate)" />
    public static Behavior Given(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.GivenInternal(name, step);

    /// <inheritdoc cref="Given(Behavior, string, Delegate)" />
    public static Behavior Given(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.GivenInternal(name, step);

    /// <inheritdoc cref="Given(Behavior, string, Delegate)" />
    public static Behavior GivenAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.GivenInternal(name, step);

    /// <inheritdoc cref="Given(Behavior, string, Delegate)" />
    public static Behavior GivenAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.GivenInternal(name, step);

    /// <inheritdoc cref="Given(Behavior, string, Delegate)" />
    public static Behavior GivenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.GivenInternal(name, step);

    /// <summary>
    /// Adds a 'Given' step to the behavior.
    /// </summary>
    /// <typeparam name="TStep">The type of step to add.</typeparam>
    /// <param name="behavior">The behavior to update.</param>
    /// <returns>The current step.</returns>
    public static Behavior Given<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.Given(new TStep());

    /// <inheritdoc cref="Given(Behavior, Delegate)" />
    public static Behavior Given(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

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

    /// <summary>
    /// Adds a 'And' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior And(this Behavior behavior, Delegate step) =>
        behavior.And(step.Method.Name, step);

    /// <inheritdoc cref="And(Behavior, Delegate)" />
    public static Behavior And(this Behavior behavior, Action step) =>
        behavior.And(step.Method.Name, step);

    /// <inheritdoc cref="And(Behavior, Delegate)" />
    public static Behavior And(this Behavior behavior, Action<dynamic> step) =>
        behavior.And(step.Method.Name, step);

    /// <inheritdoc cref="And(Behavior, Delegate)" />
    public static Behavior And(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.And(step.Method.Name, step);

    /// <inheritdoc cref="And(Behavior, Delegate)" />
    public static Behavior AndAsync(this Behavior behavior, Func<Task> step) =>
        behavior.AndAsync(step.Method.Name, step);

    /// <inheritdoc cref="And(Behavior, Delegate)" />
    public static Behavior AndAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.AndAsync(step.Method.Name, step);

    /// <inheritdoc cref="And(Behavior, Delegate)" />
    public static Behavior AndAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.AndAsync(step.Method.Name, step);

    /// <summary>
    /// Adds a 'And' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="name">The name of the step.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior And(this Behavior behavior, string name, Delegate step) =>
        behavior.AndInternal(name, step);

    /// <inheritdoc cref="And(Behavior, string, Delegate)" />
    public static Behavior And(this Behavior behavior, string name, Action step) =>
        behavior.AndInternal(name, step);

    /// <inheritdoc cref="And(Behavior, string, Delegate)" />
    public static Behavior And(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.AndInternal(name, step);

    /// <inheritdoc cref="And(Behavior, string, Delegate)" />
    public static Behavior And(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.AndInternal(name, step);

    /// <inheritdoc cref="And(Behavior, string, Delegate)" />
    public static Behavior AndAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.AndInternal(name, step);

    /// <inheritdoc cref="And(Behavior, string, Delegate)" />
    public static Behavior AndAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.AndInternal(name, step);

    /// <inheritdoc cref="And(Behavior, string, Delegate)" />
    public static Behavior AndAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.AndInternal(name, step);

    /// <summary>
    /// Adds a 'And' step to the behavior.
    /// </summary>
    /// <typeparam name="TStep">The type of step to add.</typeparam>
    /// <param name="behavior">The behavior to update.</param>
    /// <returns>The current step.</returns>
    public static Behavior And<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.And(new TStep());

    /// <inheritdoc cref="And(Behavior, Delegate)" />
    public static Behavior And(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

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

    /// <summary>
    /// Adds a 'When' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior When(this Behavior behavior, Delegate step) =>
        behavior.When(step.Method.Name, step);

    /// <inheritdoc cref="When(Behavior, Delegate)" />
    public static Behavior When(this Behavior behavior, Action step) =>
        behavior.When(step.Method.Name, step);

    /// <inheritdoc cref="When(Behavior, Delegate)" />
    public static Behavior When(this Behavior behavior, Action<dynamic> step) =>
        behavior.When(step.Method.Name, step);

    /// <inheritdoc cref="When(Behavior, Delegate)" />
    public static Behavior When(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.When(step.Method.Name, step);

    /// <inheritdoc cref="When(Behavior, Delegate)" />
    public static Behavior WhenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.WhenAsync(step.Method.Name, step);

    /// <inheritdoc cref="When(Behavior, Delegate)" />
    public static Behavior WhenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.WhenAsync(step.Method.Name, step);

    /// <inheritdoc cref="When(Behavior, Delegate)" />
    public static Behavior WhenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.WhenAsync(step.Method.Name, step);

    /// <summary>
    /// Adds a 'When' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="name">The name of the step.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior When(this Behavior behavior, string name, Delegate step) =>
        behavior.WhenInternal(name, step);

    /// <inheritdoc cref="When(Behavior, string, Delegate)" />
    public static Behavior When(this Behavior behavior, string name, Action step) =>
        behavior.WhenInternal(name, step);

    /// <inheritdoc cref="When(Behavior, string, Delegate)" />
    public static Behavior When(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.WhenInternal(name, step);

    /// <inheritdoc cref="When(Behavior, string, Delegate)" />
    public static Behavior When(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.WhenInternal(name, step);

    /// <inheritdoc cref="When(Behavior, string, Delegate)" />
    public static Behavior WhenAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.WhenInternal(name, step);

    /// <inheritdoc cref="When(Behavior, string, Delegate)" />
    public static Behavior WhenAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.WhenInternal(name, step);

    /// <inheritdoc cref="When(Behavior, string, Delegate)" />
    public static Behavior WhenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.WhenInternal(name, step);

    /// <summary>
    /// Adds a 'When' step to the behavior.
    /// </summary>
    /// <typeparam name="TStep">The type of step to add.</typeparam>
    /// <param name="behavior">The behavior to update.</param>
    /// <returns>The current step.</returns>
    public static Behavior When<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.When(new TStep());

    /// <inheritdoc cref="When(Behavior, Delegate)" />
    public static Behavior When(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

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

    /// <summary>
    /// Adds a 'Then' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior Then(this Behavior behavior, Delegate step) =>
        behavior.Then(step.Method.Name, step);

    /// <inheritdoc cref="Then(Behavior, Delegate)" />
    public static Behavior Then(this Behavior behavior, Action step) =>
        behavior.Then(step.Method.Name, step);

    /// <inheritdoc cref="Then(Behavior, Delegate)" />
    public static Behavior Then(this Behavior behavior, Action<dynamic> step) =>
        behavior.Then(step.Method.Name, step);

    /// <inheritdoc cref="Then(Behavior, Delegate)" />
    public static Behavior Then(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.Then(step.Method.Name, step);

    /// <inheritdoc cref="Then(Behavior, Delegate)" />
    public static Behavior ThenAsync(this Behavior behavior, Func<Task> step) =>
        behavior.ThenAsync(step.Method.Name, step);

    /// <inheritdoc cref="Then(Behavior, Delegate)" />
    public static Behavior ThenAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.ThenAsync(step.Method.Name, step);

    /// <inheritdoc cref="Then(Behavior, Delegate)" />
    public static Behavior ThenAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.ThenAsync(step.Method.Name, step);

    /// <summary>
    /// Adds a 'Then' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="name">The name of the step.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior Then(this Behavior behavior, string name, Delegate step) =>
        behavior.ThenInternal(name, step);

    /// <inheritdoc cref="Then(Behavior, string, Delegate)" />
    public static Behavior Then(this Behavior behavior, string name, Action step) =>
        behavior.ThenInternal(name, step);

    /// <inheritdoc cref="Then(Behavior, string, Delegate)" />
    public static Behavior Then(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.ThenInternal(name, step);

    /// <inheritdoc cref="Then(Behavior, string, Delegate)" />
    public static Behavior Then(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.ThenInternal(name, step);

    /// <inheritdoc cref="Then(Behavior, string, Delegate)" />
    public static Behavior ThenAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.ThenInternal(name, step);

    /// <inheritdoc cref="Then(Behavior, string, Delegate)" />
    public static Behavior ThenAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.ThenInternal(name, step);

    /// <inheritdoc cref="Then(Behavior, string, Delegate)" />
    public static Behavior ThenAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.ThenInternal(name, step);

    /// <summary>
    /// Adds a 'Then' step to the behavior.
    /// </summary>
    /// <typeparam name="TStep">The type of step to add.</typeparam>
    /// <param name="behavior">The behavior to update.</param>
    /// <returns>The current step.</returns>
    public static Behavior Then<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.Then(new TStep());

    /// <inheritdoc cref="Then(Behavior, Delegate)" />
    public static Behavior Then(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

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

    /// <summary>
    /// Adds a 'But' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior But(this Behavior behavior, Delegate step) =>
        behavior.But(step.Method.Name, step);

    /// <inheritdoc cref="But(Behavior, Delegate)" />
    public static Behavior But(this Behavior behavior, Action step) =>
        behavior.But(step.Method.Name, step);

    /// <inheritdoc cref="But(Behavior, Delegate)" />
    public static Behavior But(this Behavior behavior, Action<dynamic> step) =>
        behavior.But(step.Method.Name, step);

    /// <inheritdoc cref="But(Behavior, Delegate)" />
    public static Behavior But(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.But(step.Method.Name, step);

    /// <inheritdoc cref="But(Behavior, Delegate)" />
    public static Behavior ButAsync(this Behavior behavior, Func<Task> step) =>
        behavior.ButAsync(step.Method.Name, step);

    /// <inheritdoc cref="But(Behavior, Delegate)" />
    public static Behavior ButAsync(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.ButAsync(step.Method.Name, step);

    /// <inheritdoc cref="But(Behavior, Delegate)" />
    public static Behavior ButAsync(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.ButAsync(step.Method.Name, step);

    /// <summary>
    /// Adds a 'But' step to the behavior.
    /// </summary>
    /// <param name="behavior">The behavior to update.</param>
    /// <param name="name">The name of the step.</param>
    /// <param name="step">The step to add.</param>
    /// <returns>The current step.</returns>
    public static Behavior But(this Behavior behavior, string name, Delegate step) =>
        behavior.ButInternal(name, step);

    /// <inheritdoc cref="But(Behavior, string, Delegate)" />
    public static Behavior But(this Behavior behavior, string name, Action step) =>
        behavior.ButInternal(name, step);

    /// <inheritdoc cref="But(Behavior, string, Delegate)" />
    public static Behavior But(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.ButInternal(name, step);

    /// <inheritdoc cref="But(Behavior, string, Delegate)" />
    public static Behavior But(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.ButInternal(name, step);

    /// <inheritdoc cref="But(Behavior, string, Delegate)" />
    public static Behavior ButAsync(this Behavior behavior, string name, Func<Task> step) =>
        behavior.ButInternal(name, step);

    /// <inheritdoc cref="But(Behavior, string, Delegate)" />
    public static Behavior ButAsync(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.ButInternal(name, step);

    /// <inheritdoc cref="But(Behavior, string, Delegate)" />
    public static Behavior ButAsync(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.ButInternal(name, step);

    /// <summary>
    /// Adds a 'But' step to the behavior.
    /// </summary>
    /// <typeparam name="TStep">The type of step to add.</typeparam>
    /// <param name="behavior">The behavior to update.</param>
    /// <returns>The current step.</returns>
    public static Behavior But<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.But(new TStep());

    /// <inheritdoc cref="But(Behavior, Delegate)" />
    public static Behavior But(this Behavior behavior, IStep step)
    {
        behavior.AddStep(step);
        return behavior;
    }

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
