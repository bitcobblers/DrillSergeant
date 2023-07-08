using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

// ReSharper disable once CheckNamespace
namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method)]
[TraitDiscoverer("DrillSergeant.Xunit2.BehaviorTraitDiscoverer", "DrillSergeant.Xunit2")]
[XunitTestCaseDiscoverer("DrillSergeant.Xunit2.BehaviorDiscoverer", "DrillSergeant.Xunit2")]
public sealed class BehaviorAttribute : FactAttribute, ITraitAttribute
{
    public string? Feature { get; set; }
}
