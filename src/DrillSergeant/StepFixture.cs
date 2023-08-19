namespace DrillSergeant;

/// <summary>
/// Defines an executable fixture that acts as a step..
/// </summary>
/// <typeparam name="T">The return type for the fixture.</typeparam>
public abstract class StepFixture<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StepFixture{T}"/>
    /// </summary>
    /// <param name="name">The name of the fixture.</param>
    protected StepFixture(string? name)
    {
        Name = string.IsNullOrWhiteSpace(name) ?
            GetType().Name :
            name;
    }

    /// <summary>
    /// Gets the name of the fixture.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Executes the fixture.
    /// </summary>
    /// <returns>The result of the fixture.</returns>
    public abstract T Execute();
}
