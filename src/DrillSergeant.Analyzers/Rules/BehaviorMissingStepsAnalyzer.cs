using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DrillSergeant.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BehaviorMissingStepsAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DS0002";

    private const string Title = "Behavior method does not add any steps";
    private const string MessageFormat = "Behavior method has no defined steps and will not do anything";
    private const string Description = "At least one step should be defined within a behavior.";
    private const string Category = "Design";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCodeBlockAction(AnalyzeBlock);
    }

    private void AnalyzeBlock(CodeBlockAnalysisContext context)
    {
        if (context.CodeBlock is not MethodDeclarationSyntax method)
        {
        }
    }
}