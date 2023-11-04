using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DrillSergeant.Analyzers.Rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EagerStepResultEvaluationAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DS0003";

    private const string Title = "Step results can only be evaluated within steps";
    private const string MessageFormat = "The step result '{0}' is being referenced outside of a step";
    private const string Description = "Attempting to access a step result outside of a step will result in a EagerStepResultEvaluationException being thrown.";
    private const string Category = "Usage";

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
    }
}