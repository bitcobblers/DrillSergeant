using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DrillSergeant;

/// <summary>
/// Allows access to the currently executing behavior.
/// </summary>
public static class CurrentBehavior
{
    private static readonly ReflectionParameterCaster Caster = new();
    private static readonly AsyncLocal<BehaviorState?> Instance = new();

    internal static void Set(IBehavior behavior) =>
        Instance.Value = new BehaviorState(behavior);

    internal static void Clear() => Instance.Value = null;

    internal static void ResetContext() => Instance.Value!.TrackedContext = null;

    internal static void UpdateContext()
    {
        AssertBehavior();

        if (Instance.Value!.TrackedContext == null || Instance.Value.IsTrackedContextReadonly)
        {
            return;
        }

        BaseStep.UpdateContext(Instance.Value.Behavior.Context, Instance.Value.TrackedContext);
    }

    /// <summary>
    /// Gets the current behavior context.
    /// </summary>
    public static dynamic Context
    {
        get
        {
            AssertBehavior();
            return Instance.Value!.Behavior.Context;
        }
    }

    /// <summary>
    /// Gets a copy of the current behavior input.
    /// </summary>
    public static dynamic Input
    {
        get
        {
            AssertBehavior();
            return BaseStep.CopyInput(Instance.Value!.Behavior.Input);
        }
    }

    /// <summary>
    /// Maps the current behavior context to a strongly typed object.
    /// </summary>
    /// <typeparam name="T">The type to map the context to.</typeparam>
    /// <param name="isReadonly">True if changes to the context should not be applied back to the context at the end of the step.</param>
    /// <returns>A strongly typed context.</returns>
    /// <exception cref="ContextAlreadyMappedException">Thrown if the context is mapped twice within a single step.</exception>
    public static T MapContext<T>(bool isReadonly = false)
    {
        AssertBehavior();

        if (Instance.Value!.TrackedContext != null)
        {
            throw new ContextAlreadyMappedException();
        }

        Instance.Value.IsTrackedContextReadonly = isReadonly;
        Instance.Value.TrackedContext = Caster.Cast(Instance.Value.Behavior.Context, typeof(T));

        return (T)Instance.Value.TrackedContext;
    }

    /// <summary>
    /// Maps the current behavior input to a strongly typed object.
    /// </summary>
    /// <typeparam name="T">The type to map the context to.</typeparam>
    /// <returns>A strongly typed copy of the input.</returns>
    public static T MapInput<T>()
    {
        AssertBehavior();
        return (T)Caster.Cast(Instance.Value!.CopiedInput, typeof(T));
    }

    private static void AssertBehavior([CallerMemberName] string memberName = "")
    {
        if (Instance.Value == null)
        {
            throw new NoCurrentBehaviorException(memberName);
        }
    }

    private class BehaviorState
    {
        public BehaviorState(IBehavior behavior)
        {
            Behavior = behavior;
            CopiedInput = BaseStep.CopyInput(behavior.Input);
        }

        public IBehavior Behavior { get; }
        public IDictionary<string, object?> CopiedInput { get; }
        public bool IsTrackedContextReadonly { get; set; }
        public object? TrackedContext { get; set; }
    }
}
