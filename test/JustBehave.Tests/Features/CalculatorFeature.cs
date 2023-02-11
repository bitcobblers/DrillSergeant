using JustBehave.Core;
using System;
using System.Collections.Generic;

namespace JustBehave.Tests.Features;

public class Calculator
{
    public int Add(int a, int b) => a + b;
}

public class CalculatorBehaviors
{
    public record Context(int A, int B, int Result);
    public record Input(int A, int B, int Expected);

    public IEnumerable<Input> AdditionInputs
    {
        get
        {
            yield return new Input(A: 1, B: 2, Expected: 3);
            yield return new Input(A: 2, B: 3, Expected: 5);
            yield return new Input(A: 3, B: 4, Expected: 7);
            yield return new Input(A: 4, B: 5, Expected: 9);
            yield return new Input(A: 5, B: 6, Expected: 11);
        }
    }

    [Behavior]
    public Behavior<Context> AdditionBehavior()
    {
        var calculator = new Calculator();

        return new BehaviorBuilder<Context>("")
            .WithInputs(AdditionInputs)
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