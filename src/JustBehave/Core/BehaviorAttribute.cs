using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace DrillSergeant.Core;

[ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method)]
[XunitTestCaseDiscoverer("DrillSergeant.Core.BehaviorDiscoverer", "DrillSergeant")]
public sealed class BehaviorAttribute : TheoryAttribute
{
}
