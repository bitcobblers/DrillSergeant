using Analyzer =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        DrillSergeant.Analyzers.Rules.BehaviorMethodAccessorAnalyzer>;

using Fixer =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
        DrillSergeant.Analyzers.Rules.BehaviorMethodAccessorAnalyzer,
        DrillSergeant.Analyzers.Fixes.BehaviorMethodAccessorFixProvider>;

namespace DrillSergeant.Tests.Analyzers;

public class BehaviorMethodAccessorAnalyzerTests
{
    private const string SampleCode = @"
using System;

namespace MyNamespace;

[AttributeUsage(AttributeTargets.Method)]
public class BehaviorAttribute : Attribute
{
}

public class Ignored
{
    [Behavior]
    $ACCESSOR$ void InvalidBehaviorMethod()
    {
    }
}";

    private static string GetSample(string accessor) => SampleCode.Replace("$ACCESSOR$", accessor);

    public static IEnumerable<object[]> NonPublicAccessors
    {
        get
        {
            return new[]
            {
                new[] { "private" },
                new[] { "protected" },
                new[] { "internal" }
            };
        }
    }

    [Theory]
    [MemberData(nameof(NonPublicAccessors))]
    public async Task IdentifiesNonPublicBehaviorMethods(string accessor)
    {
        // Arrange.
        var code = GetSample(accessor);

        // Act.
        var result = Analyzer.Diagnostic()
            .WithLocation(13, 5)
            .WithArguments("InvalidBehaviorMethod");

        // Assert.
        await Analyzer.VerifyAnalyzerAsync(code, result);
    }

    [Theory]
    [MemberData(nameof(NonPublicAccessors))]
    public async Task ApplyingFixReplacesNonPublicAccessorWithPublic(string accessor)
    {
        // Arrange.
        var invalidCode = GetSample(accessor);
        var expectedCode = GetSample("public");

        // Act.
        var result = Fixer.Diagnostic()
            .WithLocation(13, 5)
            .WithArguments("InvalidBehaviorMethod");

        // Assert.
        await Fixer.VerifyCodeFixAsync(invalidCode, result, expectedCode);
    }

    [Theory]
    [InlineData("protected virtual", "public virtual", 5)]
    [InlineData("virtual protected", "virtual public", 5)]
    public async Task FixingNonPublicAccessorPreservesKeywordOrder(string nonPublicModifiers, string publicModifiers, int column)
    {
        // Arrange.
        var invalidCode = GetSample(nonPublicModifiers);
        var expectedCode = GetSample(publicModifiers);

        // Act.
        var result = Fixer.Diagnostic()
            .WithLocation(13, column)
            .WithArguments("InvalidBehaviorMethod");

        // Assert.
        await Fixer.VerifyCodeFixAsync(invalidCode, result, expectedCode);
    }
}