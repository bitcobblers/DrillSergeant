using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Reflection;
using JetBrains.Annotations;

namespace DrillSergeant;

/// <summary>
/// Defines a behavior, encapsulating a series of steps to run as a single test.
/// </summary>
public class Behavior : IBehavior
{
    private readonly List<IStep> _steps = new();
    private readonly HashSet<IDisposable> _ownedDisposables = new();

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
    public Behavior(object? input) =>
        SetInput(input);

    /// <summary>
    /// Finalizes an instance of the <see cref="Behavior"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    ~Behavior() => Dispose(disposing: false);

    /// <inheritdoc cref="IBehavior.Context" />
    public IDictionary<string, object?> Context { get; } = new ExpandoObject();

    /// <inheritdoc cref="IBehavior.Input" />
    public IDictionary<string, object?> Input { get; } = new ExpandoObject();

    /// <inheritdoc cref="IBehavior.LogContext" />
    public bool LogContext { get; private set; }

    internal ISet<IDisposable> OwnedDisposables => _ownedDisposables;

    /// <summary>
    /// Adds a new step to the behavior.
    /// </summary>
    /// <param name="step">An instance of the step to add.</param>
    /// <returns>The current behavior.</returns>
    [PublicAPI]
    public Behavior AddStep(IStep? step)
    {
        if (step != null)
        {
            _steps.Add(step);
        }

        return this;
    }

    /// <summary>
    /// Sets the input to use for the behavior.
    /// </summary>
    /// <param name="input">The input to use.</param>
    /// <returns>The current behavior.</returns>
    [PublicAPI]
    public Behavior SetInput(object? input)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        input ??= new { };
        Input.Clear();

        foreach (var property in input.GetType().GetProperties(flags))
        {
            Input[property.Name] = property.GetValue(input);
        }

        return this;
    }

    /// <summary>
    /// Sets the input to use for the behavior.
    /// </summary>
    /// <param name="input">A dictionary containing key-value input values to use.</param>
    /// <returns>The current behavior.</returns>
    [PublicAPI]
    public Behavior SetInput(IDictionary<string, object?>? input)
    {
        input ??= new Dictionary<string, object?>();
        Input.Clear();

        foreach (var (k, v) in input)
        {
            Input[k] = v;
        }

        return this;
    }

    /// <summary>
    /// Adds a background <see cref="Behavior"/> to the current behavior.
    /// </summary>
    /// <param name="background">The background behavior to add.</param>
    /// <returns>The current behavior.</returns>
    [PublicAPI]
    public Behavior Background(Behavior? background)
    {
        if (background == null)
        {
            return this;
        }

        _steps.AddRange(background);

        foreach (var disposable in background._ownedDisposables)
        {
            _ownedDisposables.Add(disposable);
        }

        background._ownedDisposables.Clear();

        return this;
    }

    /// <summary>
    /// Enables context logging between steps.
    /// </summary>
    /// <returns>The current behavior.</returns>
    [PublicAPI]
    public Behavior EnableContextLogging()
    {
        LogContext = true;
        return this;
    }

    /// <summary>
    /// Marks a disposable object as being owned by the behavior.
    /// </summary>
    /// <param name="instance">The object instance to take ownership of.</param>
    /// <returns>The current behavior.</returns>
    [PublicAPI]
    public Behavior Owns(IDisposable? instance)
    {
        if (instance != null)
        {
            _ownedDisposables.Add(instance);
        }

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

            foreach (var disposable in _ownedDisposables)
            {
                disposable.Dispose();
            }
        }

        _disposed = true;
    }
}
