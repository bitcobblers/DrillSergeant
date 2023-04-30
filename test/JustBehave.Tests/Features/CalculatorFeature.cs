using JustBehave.Core;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace JustBehave.Tests.Features;

public class CalculatorBehaviors
{
    private readonly ITestOutputHelper output;

    public record Context(int A, int B, int Result);

    public record struct Input(int A, int B, int Expected) : IXunitSerializable
    {
        public void Deserialize(IXunitSerializationInfo info)
        {
            A = info.GetValue<int>(nameof(A));
            B = info.GetValue<int>(nameof(B));
            Expected = info.GetValue<int>(nameof(Expected));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(A), A);
            info.AddValue(nameof(B), B);
            info.AddValue(nameof(Expected), Expected);
        }
    }

    public CalculatorBehaviors(ITestOutputHelper output)
    {
        this.output = output;
    }

    public static IEnumerable<object[]> AdditionInputs
    {
        get => new[] {
            new Input(1, 2, 3),
            new Input(2, 3, 5)
        }.ToObjectArray();
    }

    [Behavior, MemberData(nameof(AdditionInputs))]
    public Behavior<Context> AdditionBehavior([Inject]Calculator calculator)
    {
        this.output.WriteLine("Invoking AdditionBehavior()");

        return new BehaviorBuilder<Context>("")
            .WithInput<Input>()
            //.WithInput(() => new { })
            .Given("Set first number", (c, i) => c with { A = i.A }) // Inline step declaration.
            .Given(SetSecondNumber)
            .When(AddNumbers(calculator))
            .Then<CheckResultStep>()
            .Build();
    }

    // Step implemented as a normal method.
    public Context SetSecondNumber(Context context, Input input) => context with { B = input.B };

    // Step implemented as a lambda step for greater flexibility.
    public WhenStep<Context, Input, int> AddNumbers(Calculator calculator) => new LambdaWhenStep<Context, Input, int>()
        .Named("Add numbers")
        .Handle((c, _) => calculator.Add(c.A, c.B))
        .Teardown(() => Console.WriteLine("I do cleanup"));

    // Step implemented as type for full customization and reusability.
    public class CheckResultStep : ThenStep<Context, Input, int>
    {
        public override void Then(Context context, Input input, int result)
        {
            Console.WriteLine($"{input.Expected} == {result}: {input.Expected == result}");
        }
    }
}