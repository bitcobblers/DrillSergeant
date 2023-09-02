using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DrillSergeant.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BehaviorMethodScopeAnalyzer : BehaviorMethodAnalyzer
{
    public const string StaticDiagnosticId = "DS0004";

    private static readonly DiagnosticDescriptor Rule = new(
        StaticDiagnosticId,
        title: "Behavior methods cannot be static",
        messageFormat: "Behavior method '{0}' must cannot be static",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "DrillSergeant will only execute behavior methods that are public instance.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    protected override void AnalyzeSignature(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax methodDeclaration,
        IMethodSymbol method)
    {
        if (method.DeclaredAccessibility != Accessibility.Public ||
            HasBehaviorAttribute(method.GetAttributes()) == false)
        {
            return;
        }

        // ReSharper disable once InvertIf
        if (method.IsStatic)
        {
            var diag = Diagnostic.Create(
                Rule,
                methodDeclaration.GetLocation(),
                method.Name);

            context.ReportDiagnostic(diag);
        }
    }
}
