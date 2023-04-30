using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace JustBehave.Core;

[ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method)]
[XunitTestCaseDiscoverer("JustBehave.Core.BehaviorDiscoverer", "JustBehave")]
public sealed class BehaviorAttribute : TheoryAttribute
{
}
