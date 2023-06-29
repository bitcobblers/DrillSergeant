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
                    context.AddSource($"{verbGroup}/{verb}Step.g.cs", GetVerbStepTemplate(ns, verb));
                    context.AddSource($"{verbGroup}/{verb}LambdaStep.g.cs", GetLambdaVerbStepTemplate(ns, verb));
                    context.AddSource($"{verbGroup}/BehaviorExtensions.{verb}.g.cs", GetBehaviorExtensionsTemplate(ns, verb));
                }
            }
        });
    }

    private static string GetVerbStepTemplate(string ns, string name) => $@"
namespace {ns};

public class {name}Step : VerbStep
{{
    public {name}Step()
        : base(""{name}"")
    {{
    }}
}}";

    private static string GetLambdaVerbStepTemplate(string ns, string name) => $@"
namespace {ns};

public class {name}LambdaStep : LambdaStep
{{
    public {name}LambdaStep()
        : base(""{name}"")
    {{
    }}
}}
";

    private static string GetBehaviorExtensionsTemplate(string ns, string name) => $@"
using System;
using System.Threading.Tasks;

namespace {ns};

public static partial class BehaviorExtensions
{{
    public static Behavior {name}(this Behavior behavior, Action step) =>
        behavior.{name}(step.Method.Name, step);

    public static Behavior {name}(this Behavior behavior, Action<dynamic> step) =>
        behavior.{name}(step.Method.Name, step);

    public static Behavior {name}(this Behavior behavior, Action<dynamic, dynamic> step) =>
        behavior.{name}(step.Method.Name, step);

    public static Behavior {name}<TContext>(this Behavior behavior, Action<TContext> step) =>
        behavior.{name}(step.Method.Name, step);

    public static Behavior {name}<TInput>(this Behavior behavior, Action<dynamic, TInput> step) =>
        behavior.{name}(step.Method.Name, step);

    public static Behavior {name}<TContext, TInput>(this Behavior behavior, Action<TContext, TInput> step) =>
        behavior.{name}(step.Method.Name, step);

    // ---

    public static Behavior {name}Async(this Behavior behavior, Func<Task> step) =>
        behavior.{name}Internal(step.Method.Name, step);

    public static Behavior {name}Async(this Behavior behavior, Func<dynamic, Task> step) =>
        behavior.{name}Internal(step.Method.Name, step);

    public static Behavior {name}Async(this Behavior behavior, Func<dynamic, dynamic, Task> step) =>
        behavior.{name}Internal(step.Method.Name, step);

    public static Behavior {name}Async<TContext>(this Behavior behavior, Func<TContext, Task> step) =>
        behavior.{name}Internal(step.Method.Name, step);

    public static Behavior {name}Async<TInput>(this Behavior behavior, Func<dynamic, TInput, Task> step) =>
        behavior.{name}Internal(step.Method.Name, step);

    public static Behavior {name}Async<TContext, TInput>(this Behavior behavior, Func<TContext, TInput, Task> step) =>
        behavior.{name}Internal(step.Method.Name, step);

    // ---

    public static Behavior {name}(this Behavior behavior, string name, Action step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}(this Behavior behavior, string name, Action<dynamic> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}(this Behavior behavior, string name, Action<dynamic, dynamic> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}<TContext>(this Behavior behavior, string name, Action<TContext> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}<TInput>(this Behavior behavior, string name, Action<dynamic, TInput> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}<TContext, TInput>(this Behavior behavior, string name, Action<TContext, TInput> step) =>
        behavior.{name}Internal(name, step);

    // ---

    public static Behavior {name}Async(this Behavior behavior, string name, Func<Task> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}Async(this Behavior behavior, string name, Func<dynamic, Task> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}Async(this Behavior behavior, string name, Func<dynamic, dynamic, Task> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}Async<TContext>(this Behavior behavior, string name, Func<TContext, Task> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}Async<TInput>(this Behavior behavior, string name, Func<dynamic, TInput, Task> step) =>
        behavior.{name}Internal(name, step);

    public static Behavior {name}Async<TContext, TInput>(this Behavior behavior, string name, Func<TContext, TInput, Task> step) =>
        behavior.{name}Internal(name, step);

    // ---

    public static Behavior {name}<TStep>(this Behavior behavior) where TStep : IStep, new() =>
        behavior.{name}(new TStep());

    public static Behavior {name}(this Behavior behavior, IStep step)
    {{
        behavior.AddStep(step);
        return behavior;
    }}

    // ---

    private static Behavior {name}Internal(this Behavior behavior, string name, Delegate step)
    {{
        behavior.AddStep(
            new {name}LambdaStep()
                .Named(name)
                .Handle(step));

        return behavior;
    }}
}}
";
}
