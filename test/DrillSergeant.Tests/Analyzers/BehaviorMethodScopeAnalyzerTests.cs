﻿using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        DrillSergeant.Analyzers.Rules.BehaviorMethodScopeAnalyzer>;

namespace DrillSergeant.Tests.Analyzers;

public class BehaviorMethodScopeAnalyzerTests
{
    private const string SampleWithStaticMethod = @"
using System;

namespace MyNamespace;

[AttributeUsage(AttributeTargets.Method)]
public class BehaviorAttribute : Attribute
{
}

public class Ignored
{
    [Behavior]
    public static void InvalidBehaviorMethod()
    {
    }
}";

    [Fact]
    public async Task IdentifiesMethodWithStaticMethod()
    {
        // Act.
        var result = Verifier.Diagnostic()
            .WithLocation(13, 5)
            .WithArguments("InvalidBehaviorMethod");

        // Assert.
        await Verifier.VerifyAnalyzerAsync(SampleWithStaticMethod, result);
    }
}