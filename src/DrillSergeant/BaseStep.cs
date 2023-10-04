using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DrillSergeant;

/// <summary>
/// Defines the root type that all steps are derived from.
/// </summary>
/// <remarks>
/// <para>
/// Steps deriving from this type must implement <see cref="PickHandler"/> to determine which handler to execute when running the step.
/// </para>
/// </remarks>
public abstract class BaseStep : IStep
{
    /// <inheritdoc />
    public virtual string Verb { get; protected set; } = "<unknown>";

    /// <inheritdoc />
    public virtual string Name => "<untitled step>";

    /// <inheritdoc />
    public virtual bool ShouldSkip => false;

    [ExcludeFromCodeCoverage]
    ~BaseStep()
    {
        Dispose(disposing: false);
    }

    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual async Task Execute()
    {
        var handler = PickHandler();
        var parameters = Array.Empty<object>();
        dynamic result = handler.DynamicInvoke(parameters)!;

        if (IsAsync(handler.Method))
        {
            await result;
        }
    }

    /// <summary>
    /// Picks the handler method in the step that should be executed by the test runner.
    /// </summary>
    /// <returns>A delegate to the handler that should be executed.</returns>
    protected abstract Delegate PickHandler();

    [ExcludeFromCodeCoverage]
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    // ReSharper disable once UnusedParameter.Global
    protected virtual void Dispose(bool disposing)
    {
    }

    internal static bool IsAsync(MethodInfo method) =>
        method.ReturnType.Name == nameof(Task) ||
        method.ReturnType.Name == typeof(Task<>).Name;
}
