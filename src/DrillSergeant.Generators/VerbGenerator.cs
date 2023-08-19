using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace DrillSergeant.Generators;

[Generator, ExcludeFromCodeCoverage]
public class VerbGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Uncomment the following line to debug the generator.
        // System.Diagnostics.Debugger.Launch();

        var files = context.AdditionalTextsProvider.Where(t => new FileInfo(t.Path).Name.ToLower() == "verbdefinitions.txt");
        var providers = files.Select((text, token) => text.GetText(token)!.ToString());

        context.RegisterSourceOutput(providers, static (context, text) =>
        {
            foreach (var verbGroup in VerbDefinitionParser.Parse(text))
            {
                foreach (var verb in verbGroup.Verbs)
                {
                    var className = $"{verbGroup.Name}_{verb}.g.cs";
                    var template = GetVerbGroupStaticsTemplate("DrillSergeant", verbGroup.Name, verb);

                    context.AddSource(className, template);
                }
            }
        });
    }

    private static string GetVerbGroupStaticsTemplate(string ns, string groupName, string verb) => $@"
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace {ns};

public static partial class {groupName}
{{
    [ExcludeFromCodeCoverage]
    public static void {verb}(Action step) =>
        StepBuilder.AddStep(""{verb}"", step.Method.Name, step);

    [ExcludeFromCodeCoverage]
    public static void {verb}(string name, Action step) =>
        StepBuilder.AddStep(""{verb}"", name, step);

    [ExcludeFromCodeCoverage]
    public static void {verb}Async(Func<Task> step) =>
        StepBuilder.AddStepAsync(""{verb}"", step.Method.Name, step);

    [ExcludeFromCodeCoverage]
    public static void {verb}Async(string name, Func<Task> step) =>
        StepBuilder.AddStepAsync(""{verb}"", name, step);

    // ---

    [ExcludeFromCodeCoverage]
    public static StepResult<T> {verb}<T>(Func<T> step) =>
        StepBuilder.AddStep<T>(""{verb}"", step.Method.Name, step);

    [ExcludeFromCodeCoverage]
    public static StepResult<T> {verb}<T>(string name, Func<T> step) =>
        StepBuilder.AddStep<T>(""{verb}"", name, step);

    [ExcludeFromCodeCoverage]
    public static AsyncStepResult<T> {verb}Async<T>(Func<Task<T>> step) =>
        StepBuilder.AddStepAsync<T>(""{verb}"", step.Method.Name, step);

    [ExcludeFromCodeCoverage]
    public static AsyncStepResult<T> {verb}Async<T>(string name, Func<Task<T>> step) =>
        StepBuilder.AddStepAsync<T>(""{verb}"", name, step);

    // ---

    [ExcludeFromCodeCoverage]
    public static void {verb}<TStep>() where TStep : IStep, new() =>
        StepBuilder.AddStep(""{verb}"", new TStep());

    [ExcludeFromCodeCoverage]
    public static void {verb}(IStep step) =>
        StepBuilder.AddStep(""{verb}"", step);

    // ---

    [ExcludeFromCodeCoverage]
    public static StepResult<T> {verb}<T>(StepFixture<T> fixture) =>
        StepBuilder.AddStep(""{verb}"", fixture);

    [ExcludeFromCodeCoverage]
    public static AsyncStepResult<T> {verb}<T>(AsyncStepFixture<T> fixture) =>
        StepBuilder.AddStepAsync(""{verb}"", fixture);
}}
";
}
