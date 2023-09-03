using System.Threading.Tasks;

using Analyzer =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        DrillSergeant.Analyzers.Rules.MutatingInputAnalyzer>;

namespace DrillSergeant.Tests.Analyzers;

public class MutatingInputAnalyzerTests
{
    private const string CodeTemplate =
        @"using System;

$PREFIX$

namespace DrillSergeant;

[AttributeUsage(AttributeTargets.Method)]
public class BehaviorAttribute : Attribute
{
}

public static class CurrentBehavior
{
    public static dynamic Input { get; private set; }
    public static T MapInput<T>() where T: new() => new T();
}

$CODE$";

    private static string FormatTemplate(string code) =>
        FormatTemplate(string.Empty, code);

    private static string FormatTemplate(string prefix, string code) =>
        CodeTemplate
            .Replace("$PREFIX$", prefix)
            .Replace("$CODE$", code);

    [Fact]
    public async Task CheckBasicPropertyAssignment()
    {
        // Arrange.
        var code = FormatTemplate(
            @"public class Ignored
{
    [Behavior]
    public void SampleBehavior()
    {
        CurrentBehavior.Input.MyProperty = 1;
    }
}");

        // Act.
        var result = Analyzer.Diagnostic()
            .WithSpan(23, 31, 23, 41);

        // Assert.
        await Analyzer.VerifyAnalyzerAsync(code, result);
    }

    [Fact]
    public async Task CheckFullyQualifiedPropertyAssignment()
    {
        // Arrange.
        var code = FormatTemplate(
            @"public class Ignored
{
    [Behavior]
    public void SampleBehavior()
    {
        DrillSergeant.CurrentBehavior.Input.MyProperty = 1;
    }
}");

        // Act.
        var result = Analyzer.Diagnostic()
            .WithSpan(23, 45, 23, 55);

        // Assert.
        await Analyzer.VerifyAnalyzerAsync(code, result);
    }

    [Fact]
    public async Task CheckAliasedPropertyAssignment()
    {
        // Arrange.
        var code = FormatTemplate(
            prefix: "using SubstitutedName=DrillSergeant.CurrentBehavior;",
            code:
            @"public class Ignored
{
    [Behavior]
    public void SampleBehavior()
    {
        SubstitutedName.Input.MyProperty = 1;
    }
}");

        // Act.
        var result = Analyzer.Diagnostic()
            .WithSpan(23, 31, 23, 41);

        // Assert.
        await Analyzer.VerifyAnalyzerAsync(code, result);
    }

    [Fact]
    public async Task CheckStaticAliasPropertyAssignment()
    {
        // Arrange.
        var code = FormatTemplate(
            prefix: "using static DrillSergeant.CurrentBehavior;",
            code:
            @"public class Ignored
{
    [Behavior]
    public void SampleBehavior()
    {
        Input.MyProperty = 1;
    }
}");

        // Act.
        var result = Analyzer.Diagnostic()
            .WithSpan(23, 15, 23, 25);

        // Assert.
        await Analyzer.VerifyAnalyzerAsync(code, result);
    }

    [Fact]
    public async Task CheckLocalDeclarationTakesPrecedenceOverStaticAliasPropertyAssignment()
    {
        // Arrange.
        var code = FormatTemplate(
            prefix: "using static DrillSergeant.CurrentBehavior;",
            code:
            @"public class MyInput
{
    public int MyProperty { get; set; }
}

public class Ignored
{
    [Behavior]
    public void SampleBehavior()
    {
        var Input = new MyInput();
        Input.MyProperty = 1;
    }
}");

        // Assert.
        await Analyzer.VerifyAnalyzerAsync(code);
    }
}