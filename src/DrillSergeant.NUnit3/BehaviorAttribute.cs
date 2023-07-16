using DrillSergeant.NUnit3;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Commands;

// ReSharper disable once CheckNamespace
namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class BehaviorAttribute 
    : NUnitAttribute, IWrapTestMethod, ISimpleTestBuilder, IApplyToTest, IImplyFixture
{
    private readonly NUnitTestCaseBuilder _builder = new();

    public string? Feature { get; set; }

    public TestMethod BuildFrom(IMethodInfo method, Test? suite)
    {
        return _builder.BuildTestMethod(method, suite, null);
    }

    public void ApplyToTest(Test test)
    {
        if (string.IsNullOrWhiteSpace(Feature) == false)
        {
            test.Properties.Add(PropertyNames.Category, Feature);
        }
    }

    public TestCommand Wrap(TestCommand command)
    {
        return new BehaviorCommand(command.Test);
    }
}
