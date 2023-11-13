using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using static DrillSergeant.ParameterCaster;

namespace DrillSergeant;

/// <summary>
/// Allows access to the currently executing behavior.
/// </summary>
public static class CurrentBehavior
{
    private static readonly AsyncLocal<BehaviorState?> Instance = new();

    internal static void Set(Behavior behavior) =>
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

        UpdateContext(Instance.Value.Behavior.Context, Instance.Value.TrackedContext);
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
            return CopyInput(Instance.Value!.Behavior.Input);
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
        Instance.Value.TrackedContext = Cast(Instance.Value.Behavior.Context, typeof(T));

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
        return (T)Cast(Instance.Value!.CopiedInput, typeof(T));
    }

    /// <summary>
    /// Maps the current behavior input to a strongly typed object.
    /// </summary>
    /// <param name="reference">An anonymous object describing the shape of the input to return.</param>
    /// <typeparam name="T">The type of object to map to.</typeparam>
    /// <returns>An object containing the input properties in the referenced object.</returns>
    public static T MapInput<T>(T reference) where T : class
    {
        AssertBehavior();

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;

        var args =
            from property in typeof(T).GetProperties(flags)
            let input = Instance.Value!.CopiedInput
            let hasProperty = input.ContainsKey(property.Name)
            let hasMatchingType = hasProperty && input[property.Name].GetType() == property.PropertyType
            select hasMatchingType ? input[property.Name] : null;

        var result = Activator.CreateInstance(typeof(T), args.ToArray());

        return (result as T)!;
    }

    /// <summary>
    /// Tells the current behavior to take ownership of a disposable object.
    /// </summary>
    /// <typeparam name="T">The type of object to take ownership of.</typeparam>
    /// <param name="instance">The object to take ownership of.</param>
    public static void Owns<T>(T? instance) where T : IDisposable
    {
        AssertBehavior();
        Instance.Value?.Behavior.Owns(instance);
    }

    internal static IDictionary<string, object?> CopyInput(IDictionary<string, object?> input)
    {
        var copy = new ExpandoObject();
        var copyAsDict = (IDictionary<string, object?>)copy;

        foreach ((string key, object? value) in input)
        {
            copyAsDict[key] = value;
        }

        return copy;
    }

    internal static void UpdateContext(IDictionary<string, object?> context, object changedContext)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        foreach (var property in changedContext.GetType().GetProperties(flags))
        {
            context[property.Name] = property.GetValue(changedContext);
        }
    }

    private static void AssertBehavior([CallerMemberName] string memberName = "")
    {
        if (Instance.Value == null)
        {
            throw new NoCurrentBehaviorException(memberName);
        }
    }

    internal class BehaviorState
    {
        public BehaviorState(Behavior behavior)
        {
            Behavior = behavior;
            CopiedInput = CopyInput(behavior.Input);
        }

        public Behavior Behavior { get; }
        public IDictionary<string, object?> CopiedInput { get; }
        public bool IsTrackedContextReadonly { get; set; }
        public object? TrackedContext { get; set; }
    }
}
