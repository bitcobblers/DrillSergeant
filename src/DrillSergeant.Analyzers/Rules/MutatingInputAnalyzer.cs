using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DrillSergeant.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MutatingInputAnalyzer : BehaviorMethodAnalyzer
{
    public const string StaticDiagnosticId = "DS0005";

    private static readonly DiagnosticDescriptor Rule = new(
        StaticDiagnosticId,
        title: "Behavior input is immutable and cannot be modified",
        messageFormat: "Behavior input is immutable and cannot be modified",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Behavior input cannot be modified once set.  Any changes to the input will be lost once the current step finishes executing.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax declaration, IMethodSymbol method)
    {
        if (declaration.Body == null || IsBehaviorMethod(method) == false)
        {
            return;
        }

        AnalyzeStatements(context, declaration);
    }

    private static void AnalyzeStatements(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax declaration)
    {
        var assignments = GetAssignments(declaration);

        if (assignments.Count == 0)
        {
            return;
        }

        foreach (var assignment in assignments)
        {
            if (assignment.Left is not MemberAccessExpressionSyntax left)
            {
                continue;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(left.Expression);

            if (symbol.Symbol?.ContainingType.ToString() != "DrillSergeant.CurrentBehavior" ||
                symbol.Symbol.Name != "Input")
            {
                continue;
            }

            var diag = Diagnostic.Create(
                Rule,
                left.Name.GetLocation());

            context.ReportDiagnostic(diag);
        }
    }

    private static List<AssignmentExpressionSyntax> GetAssignments(MethodDeclarationSyntax method)
    {
        var collector = new AssignmentCollector();
        collector.Visit(method.Body);

        return collector.Assignments;
    }

    private class AssignmentCollector : CSharpSyntaxWalker
    {
        public List<AssignmentExpressionSyntax> Assignments { get; } = new();

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            Assignments.Add(node);
            base.VisitAssignmentExpression(node);
        }
    }
}