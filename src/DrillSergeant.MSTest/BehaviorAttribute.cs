using System.Diagnostics.CodeAnalysis;
using DrillSergeant.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class BehaviorAttribute : TestMethodAttribute
{
    // ReSharper disable once UnusedMember.Global
    [Obsolete("Feature properties in the Behavior attribute do not work for MSTest.  Use [TestCategory] instead.")]
    [ExcludeFromCodeCoverage]
    public string? Feature { get; set; }

    public override TestResult[] Execute(ITestMethod testMethod)
    {
        var context = GetContext(testMethod);

        return base.Execute(testMethod);
    }

    private static TestContext? GetContext(ITestMethod testMethod)
    {
        return testMethod
            .GetPrivateProperty("TestMethodOptions")
            ?.GetPrivateProperty("TestContext") as TestContext;
    }
}
