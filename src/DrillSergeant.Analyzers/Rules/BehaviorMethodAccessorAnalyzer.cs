using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DrillSergeant.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BehaviorMethodAccessorAnalyzer : BehaviorMethodAnalyzer
{
    public const string DiagnosticId = "DS0001";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Behavior method does not contain a [Behavior] attribute",
        messageFormat: "Behavior method '{0}' must contain a [Behavior] attribute to be executed",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "In order for DrillSergeant to run a behavior method it must be public and contain a [Behavior] attribute.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    protected override void AnalyzeSignature(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax methodDeclaration,
        IMethodSymbol method)
    {
        if (method.IsStatic ||
            method.IsAbstract ||
            HasBehaviorAttribute(method.GetAttributes()) == false)
        {
            return;
        }

        // ReSharper disable once InvertIf
        if (method.DeclaredAccessibility != Accessibility.Public)
        {
            var diag = Diagnostic.Create(
                Rule,
                methodDeclaration.GetLocation(),
                method.Name);

            context.ReportDiagnostic(diag);
        }
    }
}