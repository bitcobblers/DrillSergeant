using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace DrillSergeant;

/// <summary>
/// Defines a behavior, encapsulating a series of steps to run as a single test.
/// </summary>
public class Behavior : IBehavior
{
    private readonly List<IStep> _steps = new();

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Behavior"/> class.
    /// </summary>
    public Behavior()
        : this(new { })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Behavior"/> class.
    /// </summary>
    /// <param name="input">The input to bind to the behavior.</param>
    public Behavior(object input)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
        dynamic castedInput = new ExpandoObject();
        var dict = (IDictionary<string, object?>)castedInput;

        foreach (var property in input.GetType().GetProperties(flags))
        {
            dict[property.Name] = property.GetValue(input);
        }

        Input = dict;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Behavior"/> class.
    /// </summary>
    ~Behavior() => Dispose(disposing:false);

    /// <inheritdoc cref="IBehavior.Context" />
    public IDictionary<string, object?> Context { get; } = new ExpandoObject();

    /// <inheritdoc cref="IBehavior.Input" />
    public IDictionary<string, object?> Input { get; }

    /// <inheritdoc cref="IBehavior.LogContext" />
    public bool LogContext { get; private set; }

    /// <summary>
    /// Adds a new step to the behavior.
    /// </summary>
    /// <param name="step">An instance of the step to add.</param>
    /// <returns>The current behavior.</returns>
    public Behavior AddStep(IStep step)
    {
        _steps.Add(step);
        return this;
    }

    /// <summary>
    /// Adds a background <see cref="Behavior"/> to the current behavior.
    /// </summary>
    /// <param name="background">The background behavior to add.</param>
    /// <returns>The current behavior.</returns>
    public Behavior Background(Behavior background)
    {
        _steps.AddRange(background);
        return this;
    }

    /// <summary>
    /// Enables context logging between steps.
    /// </summary>
    /// <returns>The current behavior.</returns>
    public Behavior EnableContextLogging()
    {
        LogContext = true;
        return this;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public IEnumerator<IStep> GetEnumerator() => _steps.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            foreach (var step in _steps)
            {
                step.Dispose();
            }
        }

        _disposed = true;
    }
}
