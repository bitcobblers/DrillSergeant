using System.Diagnostics.CodeAnalysis;


// ReSharper disable once CheckNamespace
namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method)]
public sealed class BehaviorAttribute : Attribute
{
    public string? Feature { get; set; }
}
