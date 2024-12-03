using JetBrains.Annotations;

namespace DrillSergeant;

/// <summary>
/// Defines a builder for lambda steps.
/// </summary>
/// <typeparam name="TStep">The type step being built.</typeparam>
public class LambdaStepBuilder<TStep> : BaseStep
    where TStep : LambdaStepBuilder<TStep>
{
    private string? _name;
    private Func<bool> _shouldSkip = () => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStepBuilder{TStep}"/> class.
    /// </summary>
    public LambdaStepBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStepBuilder{TStep}"/> class.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    protected LambdaStepBuilder(string? name)
    {
        SetName(name);
    }

    /// <summary>
    /// Gets the handler to execute.
    /// </summary>
    protected Delegate Handler { get; set; } = new Action(() => { });

    /// <inheritdoc />
    public override string Name => _name ?? Handler?.Method.Name ?? GetType().Name;

    /// <inheritdoc />
    public override bool ShouldSkip => _shouldSkip();

    /// <summary>
    /// Sets the name of the step.
    /// </summary>
    /// <param name="name">The name to set the step to.</param>
    /// <returns>The current builder.</returns>
    [PublicAPI]
    public TStep SetName(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            _name = name.Trim();
        }

        return (TStep)this;
    }

    /// <summary>
    /// Sets the verb of the step.
    /// </summary>
    /// <param name="verb">The verb to set the step to.</param>
    /// <returns>The current builder.</returns>
    [PublicAPI]
    public TStep SetVerb(string? verb)
    {
        if (!string.IsNullOrWhiteSpace(verb))
        {
            Verb = verb.Trim();
        }

        return (TStep)this;
    }

    /// <summary>
    /// Sets a flag indicating whether the step should be skipped.
    /// </summary>
    /// <param name="shouldSkip">An optional delegate to determine if the step should be skipped.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public TStep Skip(Func<bool>? shouldSkip = null)
    {
        _shouldSkip = shouldSkip ?? (() => true);
        return (TStep)this;
    }

    /// <inheritdoc />
    protected override Delegate PickHandler() => Handler;
}
