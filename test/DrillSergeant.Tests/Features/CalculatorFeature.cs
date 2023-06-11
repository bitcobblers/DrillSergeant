using AutoFixture.Xunit2;
using DrillSergeant.GWT;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests.Features;

public class CalculatorFeature
{
    private readonly Calculator calculator = new();

    public record Input(int A, int B, int Expected);

    public static IEnumerable<object[]> AdditionInputs
    {
        get => new[]
        {
            new object[] { 1, 2, 3 },
            new object[] { 2, 3, 5 },
            new object[] { 3, 4, 7 }
        };
    }

    [Behavior]
    [InlineAutoData]
    [InlineAutoData]
    public Behavior AdditionBehaviorWithAutoData(int a, int b)
    {
        var input = new Input(a, b, a + b);

        return new Behavior(input)
            .EnableContextLogging()
            .Given("Set first number", (c, i) => c.A = i.A) // Inline step declaration.
            .And(SetSecondNumber)
            .When(AddNumbers(calculator))
            .Then(new CheckResultStep());
    }

    [Behavior, MemberData(nameof(AdditionInputs))]
    public Task<Behavior> AsyncAdditionBehavior(int a, int b, int expected)
    {
        var behavior = new Behavior(new Input(a, b, expected))
            .Given("Set first number", (c, i) => c.A = i.A)
            .GivenAsync(SetSecondNumberAsync)
            .When(AddNumbersAsync(calculator))
            .Then(new CheckResultStepAsync());

        return Task.FromResult(behavior);
    }

    [Behavior, MemberData(nameof(AdditionInputs))]
    public Behavior AdditionBehavior(int a, int b, int expected)
    {
        var input = new Input(a, b, expected);

        return new Behavior(input)
            .EnableContextLogging()
            .Given("Set first number", (c, i) => c.A = i.A) // Inline step declaration.
            .And(SetSecondNumber)
            .When(AddNumbers(calculator))
            .Then(new CheckResultStep());
    }

    // Step implemented as a normal method.
    private void SetSecondNumber(dynamic context, Input input) => context.B = input.B;

    private Task SetSecondNumberAsync(dynamic context, dynamic input)
    {
        context.B = input.B;
        return Task.CompletedTask;
    }

    // Step implemented as a lambda step for greater flexibility.
    public LambdaStep AddNumbers(Calculator calculator) =>
        new LambdaWhenStep()
            .Named("Add numbers")
            .Handle((c) =>
            {
                c.Result = calculator.Add(c.A, c.B);
            });

    public LambdaStep AddNumbersAsync(Calculator calculator) =>
        new LambdaWhenStep()
            .Named("Add numbers")
            .Handle((c, _) =>
            {
                c.Result = calculator.Add(c.A, c.B);
                return Task.CompletedTask;
            });

    // Step implemented as type for full customization and reusability.
    public class CheckResultStep : ThenStep
    {
        public void Then(dynamic context, Input input)
        {
            Assert.Equal(input.Expected, context.Result);
        }
    }

    // Class-level step.
    public class CheckResultStepAsync : ThenStep
    {
        public Task ThenAsync(dynamic context, Input input)
        {
            Assert.Equal(input.Expected, context.Result);
            return Task.CompletedTask;
        }
    }

    public class Calculator
    {
        public int Add(int a, int b) => a + b;
    }
}
