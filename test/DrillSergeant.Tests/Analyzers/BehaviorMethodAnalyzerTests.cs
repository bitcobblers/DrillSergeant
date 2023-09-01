using System.Collections.Generic;
using System.Threading.Tasks;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<DrillSergeant.Analyzers.Rules.BehaviorMethodAccessibilityAnalyzer>;
using Fixer = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<DrillSergeant.Analyzers.Rules.BehaviorMethodAccessibilityAnalyzer, DrillSergeant.Analyzers.Fixes.BehaviorMethodAccessibilityFixProvider>;

namespace DrillSergeant.Tests.Analyzers;

public class BehaviorMethodAnalyzerTests
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
    public async Task CheckForPublicReturnCode(string accessor)
    {
        // Arrange.
        var code = GetSample(accessor);

        // Act.
        var result = Verifier.Diagnostic()
            .WithLocation(13, 5)
            .WithArguments("InvalidBehaviorMethod");

        // Assert.
        await Verifier.VerifyAnalyzerAsync(code, result);
    }

    [Theory]
    [MemberData(nameof(NonPublicAccessors))]
    public async Task FixesReturnCode(string accessor)
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
}
