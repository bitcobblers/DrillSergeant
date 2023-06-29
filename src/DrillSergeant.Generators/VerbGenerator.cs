using Microsoft.CodeAnalysis;

namespace DrillSergeant.Generators;

[Generator]
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
                var ns = $"DrillSergeant.{verbGroup.Name}";

                foreach (var verb in verbGroup.Verbs)
                {
                    context.AddSource($"{verbGroup.Name}/{verb}Step.g.cs", GetVerbStepTemplate(ns, verb));
                    context.AddSource($"{verbGroup.Name}/{verb}LambdaStep.g.cs", GetLambdaVerbStepTemplate(ns, verb));
                    context.AddSource($"{verbGroup.Name}/BehaviorExtensions.{verb}.g.cs", GetBehaviorExtensionsTemplate(ns, verb));
                }
            }
        });
    }

    private static string GetVerbStepTemplate(string ns, string verb) => $@"
namespace {ns};

public class {verb}Step : VerbStep
{{
    public {verb}Step()
        : base(""{verb}"")
    {{
    }}
}}";

    private static string GetLambdaVerbStepTemplate(string ns, string verb) => $@"
namespace {ns};

public class {verb}LambdaStep : LambdaStep
{{
    public {verb}LambdaStep()
        : base(""{verb}"")
    {{
    }}

    public {verb}LambdaStep(string name)
        : base(""{verb}"", name)
    {{
    }}
}}
";

    private static string GetBehaviorExtensionsTemplate(string ns, string verb) => $@"
using System;
using System.Threading.Tasks;

namespace {ns};

public static partial class BehaviorExtensions
{{
    public static Behavior {verb}(this Behavior behavior, Action step) =>
        behavior.{verb}(step.Method.Name, step);

    public static Behavior {verb}(this Behavior behavior, Action<dynamic> step) =>
        behavior.{verb}(step.Method.Name, step);

    public static Behavior {verb}(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.{verb}(step.Method.Name, step);

    public static Behavior {verb}<TContext>(this Behavior behavior, Action<TContext> step) =>
        behavior.{verb}(step.Method.Name, step);

    public static Behavior {verb}<TInput>(this Behavior behavior, Action<dynamic, TInput> step) =>
        behavior.{verb}(step.Method.Name, step);

    public static Behavior {verb}<TContext, TInput>(this Behavior behavior, Action<TContext, TInput> step) =>
        behavior.{verb}(step.Method.Name, step);

    // ---

    public static Behavior {verb}Async(this Behavior behavior, Func<Task> step) =>
        behavior.{verb}Internal(step.Method.Name, step);

    public static Behavior {verb}Async(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.{verb}Internal(step.Method.Name, step);

    public static Behavior {verb}Async(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.{verb}Internal(step.Method.Name, step);

    public static Behavior {verb}Async<TContext>(this Behavior behavior, Func<TContext, Task> step) =>
        behavior.{verb}Internal(step.Method.Name, step);

    public static Behavior {verb}Async<TInput>(this Behavior behavior, Func<dynamic, TInput, Task> step) =>
        behavior.{verb}Internal(step.Method.Name, step);

    public static Behavior {verb}Async<TContext, TInput>(this Behavior behavior, Func<TContext, TInput, Task> step) =>
        behavior.{verb}Internal(step.Method.Name, step);

    // ---

    public static Behavior {verb}(this Behavior behavior, string name, Action step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}<TContext>(this Behavior behavior, string name, Action<TContext> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}<TInput>(this Behavior behavior, string name, Action<dynamic, TInput> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}<TContext, TInput>(this Behavior behavior, string name, Action<TContext, TInput> step) =>
        behavior.{verb}Internal(name, step);

    // ---

    public static Behavior {verb}Async(this Behavior behavior, string name, Func<Task> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}Async(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}Async(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}Async<TContext>(this Behavior behavior, string name, Func<TContext, Task> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}Async<TInput>(this Behavior behavior, string name, Func<dynamic, TInput, Task> step) =>
        behavior.{verb}Internal(name, step);

    public static Behavior {verb}Async<TContext, TInput>(this Behavior behavior, string name, Func<TContext, TInput, Task> step) =>
        behavior.{verb}Internal(name, step);

    // ---

    public static Behavior {verb}<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.{verb}(new TStep());

    public static Behavior {verb}(this Behavior behavior, IStep step)
    {{
        behavior.AddStep(step);
        return behavior;
    }}

    // ---

    private static Behavior {verb}Internal(this Behavior behavior, string name, Delegate step)
    {{
        behavior.AddStep(
            new {verb}LambdaStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }}
}}
";
}
