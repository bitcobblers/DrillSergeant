using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DrillSergeant.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BehaviorMethodAccessibilityAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DS0001";
    private const string Title = "Behavior method does not contain a [Behavior] attribute";
    private const string MessageFormat = "Behavior method '{0}' must contain a [Behavior] attribute to be executed";
    private const string Description = "In order for DrillSergeant to run a behavior method it must be public and contain a [Behavior] attribute.";
    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax methodDeclaration)
        {
            return;
        }

        var method = context.SemanticModel.GetDeclaredSymbol(methodDeclaration)!;
        var attributes = method.GetAttributes();
        var hasBehaviorAttribute = attributes.Any(x =>
        {
            var attrClass = x.AttributeClass;
            return attrClass?.Name == "BehaviorAttribute";
        });

        if (hasBehaviorAttribute == false)
        {
            return;
        }

        if (method.DeclaredAccessibility == Accessibility.Public)
        {
            return;
        }

        var diag = Diagnostic.Create(
            Rule,
            methodDeclaration.GetLocation(),
            method.Name);

        context.ReportDiagnostic(diag);
    }
}