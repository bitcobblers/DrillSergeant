using System.Diagnostics.CodeAnalysis;
using DrillSergeant.NUnit3;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

// ReSharper disable once CheckNamespace
namespace DrillSergeant;

/// <summary>
/// Defines an attribute used to notify the test runner that the method is a behavior test.
/// </summary>
[ExcludeFromCodeCoverage, AttributeUsage(AttributeTargets.Method)]
public sealed class BehaviorAttribute : TestAttribute, IWrapTestMethod
{
    public string? Feature { get; set; }
    public TestCommand Wrap(TestCommand command) => new BehaviorCommand(command.Test);
}