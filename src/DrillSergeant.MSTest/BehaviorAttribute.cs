using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class BehaviorAttribute : TestMethodAttribute 
{
    public string? Feature { get; set; }
}
