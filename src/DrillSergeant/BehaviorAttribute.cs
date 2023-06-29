using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method)]
[TraitDiscoverer("DrillSergeant.Core.BehaviorTraitDiscoverer", "DrillSergeant")]
[XunitTestCaseDiscoverer("DrillSergeant.Core.BehaviorDiscoverer", "DrillSergeant")]
public sealed class BehaviorAttribute : FactAttribute, ITraitAttribute
{
    public string? Category { get; set; }
}
