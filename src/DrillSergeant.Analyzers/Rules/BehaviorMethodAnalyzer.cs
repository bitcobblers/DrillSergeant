using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DrillSergeant.Analyzers.Rules;

public abstract class BehaviorMethodAnalyzer : DiagnosticAnalyzer
{
    private static readonly SyntaxKind[] MatchingSyntax = { SyntaxKind.MethodDeclaration };

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            syntaxKinds: MatchingSyntax,
            action: c =>
            {
                var declaration = (MethodDeclarationSyntax)c.Node;
                var method = c.SemanticModel.GetDeclaredSymbol(declaration)!;

                Analyze(c, declaration, method);
            });
    }

    protected abstract void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax declaration, IMethodSymbol method);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool IsBehaviorMethod(IMethodSymbol method) =>
        // ReSharper disable once MergeIntoPattern
        method.DeclaredAccessibility == Accessibility.Public &&
        method.IsStatic == false &&
        HasBehaviorAttribute(method.GetAttributes());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static bool HasBehaviorAttribute(ImmutableArray<AttributeData> attributes)
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var attr in attributes)
        {
            if (attr.AttributeClass?.Name == "BehaviorAttribute")
            {
                return true;
            }
        }

        return false;
    }
}