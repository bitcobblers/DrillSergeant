﻿using System.Diagnostics.CodeAnalysis;
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
                var ns = $"DrillSergeant.Syntax.{verbGroup.Name}";

                foreach (var verb in verbGroup.Verbs)
                {
                    context.AddSource($"{verbGroup.Name}/Syntax/{verb}Step.g.cs", GetVerbStepTemplate(ns, verb));
                    context.AddSource($"{verbGroup.Name}/Syntax/{verb}LambdaStep.g.cs", GetLambdaVerbStepTemplate(ns, verb));
                    context.AddSource($"{verbGroup.Name}_{verb}.g.cs", GetVerbGroupStaticsTemplate("DrillSergeant", verbGroup.Name, verb));
                }
            }
        });
    }

    private static string GetVerbStepTemplate(string ns, string verb) => $@"
// <auto-generated />
#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace {ns};

[ExcludeFromCodeCoverage]
public class {verb}Step : VerbStep
{{
    public {verb}Step()
    {{
    }}

    public {verb}Step(string? name)
        : base(name)
    {{
    }}

    public override string Verb => ""{verb}"";
}}";

    private static string GetLambdaVerbStepTemplate(string ns, string verb) => $@"
// <auto-generated />
using System.Diagnostics.CodeAnalysis;

namespace {ns};

[ExcludeFromCodeCoverage]
public class {verb}LambdaStep : LambdaStep
{{
    public {verb}LambdaStep()
    {{
        SetVerb(""{verb}"");
    }}

    public {verb}LambdaStep(string name)
        : base(name)
    {{
        SetVerb(""{verb}"");
    }}
}}
";
    
    private static string GetVerbGroupStaticsTemplate(string ns, string groupName, string verb) => $@"
// <auto-generated />
using System.Diagnostics.CodeAnalysis;
using System;
using System.Threading.Tasks;
using DrillSergeant.Syntax.{groupName};

namespace {ns};

public static partial class {groupName}
{{
    public static StepResult<T> {verb}_Ex<T>(Func<T> step) => {verb}_Ex(step.Method.Name, step);
    public static void {verb}_Ex(Action step) => {verb}_Ex(step.Method.Name, step);

    public static StepResult<T> {verb}_Ex<T>(string name, Func<T> step)
    {{
        var result = new StepResult<T>(name);

        BehaviorBuilder.Current.AddStep(
            new LambdaStep()
                .SetName(name)
                .SetVerb(""{verb}"")
                .Handle(() =>
                {{
                    var value = step();
                    result.SetResult(() => value);
                }}));

        return result;
    }}

    public static AsyncStepResult<T> {verb}Async_Ex<T>(string name, Func<Task<T>> step)
    {{
        var result = new AsyncStepResult<T>(name);

        BehaviorBuilder.Current.AddStep(
            new LambdaStep()
                .SetName(name)
                .SetVerb(""{verb}"")
                .HandleAsync(async () =>
                {{
                    var value = await step();
                    result.SetResult(() => value);
                }}));

        return result;
    }}

    public static void {verb}_Ex(string name, Action step)
    {{
        BehaviorBuilder.Current.AddStep(
            new LambdaStep()
                .SetName(name)
                .SetVerb(""{verb}"")
                .Handle(() =>
                {{
                    step();
                }}));
    }}

    public static void {verb}Async_Ex(string name, Func<Task> step)
    {{
        BehaviorBuilder.Current.AddStep(
            new LambdaStep()
                .SetName(name)
                .SetVerb(""{verb}"")
                .HandleAsync(async () =>
                {{
                    await step();
                }}));
    }}

    public static void {verb}(Action step) =>
        {verb}(step.Method.Name, step);

    [ExcludeFromCodeCoverage]
    public static void {verb}Async(Func<Task> step) =>
        {verb}Async(step.Method.Name, step);

    [ExcludeFromCodeCoverage]
    public static void {verb}(string name, Action step) =>
        BehaviorBuilder.Current.AddStep(
            new {verb}LambdaStep()
                .SetName(""{verb}"")
                .Handle(step));

    [ExcludeFromCodeCoverage]
    public static void {verb}Async(string name, Func<Task> step) =>
        BehaviorBuilder.Current.AddStep(
            new {verb}LambdaStep()
                .SetName(""{verb}"")
                .HandleAsync(step));

    // ---

    [ExcludeFromCodeCoverage]
    public static void {verb}<TStep>() where TStep : IStep, new() =>
        BehaviorBuilder.Current.AddStep(new TStep());

    [ExcludeFromCodeCoverage]
    public static void {verb}(IStep step)
    {{
        if(step is LambdaStep lambda)
        {{
            lambda.SetVerb(""{verb}"");
        }}

        BehaviorBuilder.Current.AddStep(step);
    }}
}}
";
}
