using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

// ReSharper disable once CheckNamespace
namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method)]
[TraitDiscoverer("DrillSergeant.Xunit.BehaviorTraitDiscoverer", "DrillSergeant.Xunit")]
[XunitTestCaseDiscoverer("DrillSergeant.Xunit.BehaviorDiscoverer", "DrillSergeant.Xunit")]
public sealed class BehaviorAttribute : FactAttribute, ITraitAttribute
{
    public string? Feature { get; set; }
}
