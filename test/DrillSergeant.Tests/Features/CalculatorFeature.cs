using AutoFixture.Xunit2;
using DrillSergeant.GWT;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests.Features;

public class CalculatorBehaviors
{
    private readonly Calculator calculator = new();

    public record Input(int A, int B, int Expected);

    public static IEnumerable<object[]> AdditionInputs
    {
        get => new[]
        {
            new object[] { 1, 2, 3 },
            new object[] { 2, 3, 5 }
        };
    }

    [Behavior]
    [InlineAutoData]
    [InlineAutoData]
    public IBehavior AdditionBehaviorWithAutoData(int a, int b)
    {
        var input = new Input(a, b, a + b);

        return new Behavior<Input>(input)
            .EnableContextLogging()
            .Given("Set first number", (c, i) => c.A = i.A) // Inline step declaration.
            .Given(SetSecondNumber)
            .When(AddNumbers(calculator))
            .Then(new CheckResultStep());
    }

    [Behavior, MemberData(nameof(AdditionInputs))]
    public Task<Behavior<Input>> AsyncAdditionBehavior(int a, int b, int expected)
    {
        var behavior = new Behavior<Input>(new Input(a, b, expected))
            .Given("Set first number", (c, i) => c.A = i.A)
            .Given(SetSecondNumberAsync)
            .When(AddNumbersAsync(calculator))
            .Then(new CheckResultStepAsync());

        return Task.FromResult(behavior);
    }

    [Behavior, MemberData(nameof(AdditionInputs))]
    public IBehavior AdditionBehavior(int a, int b, int expected)
    {
        var input = new Input(a, b, expected);

        return new Behavior<Input>(input)
            .EnableContextLogging()
            .Given("Set first number", (c, i) => c.A = i.A) // Inline step declaration.
            .Given(SetSecondNumber)
            .When(AddNumbers(calculator))
            .Then(new CheckResultStep());
    }

    // Step implemented as a normal method.
    private void SetSecondNumber(dynamic context, Input input) => context.B = input.B;

    private Task SetSecondNumberAsync(dynamic context, Input input)
    {
        context.B = input.B;
        return Task.CompletedTask;
    }

    // Step implemented as a lambda step for greater flexibility.
    public LambdaStep<Input> AddNumbers(Calculator calculator) =>
        new LambdaWhenStep<Input>()
            .Named("Add numbers")
            .Handle((c) =>
            {
                c.Result = calculator.Add(c.A, c.B);
            });

    public LambdaStep<Input> AddNumbersAsync(Calculator calculator) =>
        new LambdaWhenStep<Input>()
            .Named("Add numbers")
            .Handle((c, _) =>
            {
                c.Result = calculator.Add(c.A, c.B);
                return Task.CompletedTask;
            });

    // Step implemented as type for full customization and reusability.
    public class CheckResultStep : ThenStep<Input>
    {
        public void Then(dynamic context, Input input)
        {
            Assert.Equal(input.Expected, context.Result);
        }
    }

    // Class-level step.
    public class CheckResultStepAsync : ThenStep<Input>
    {
        public Task ThenAsync(dynamic context, Input input)
        {
            Assert.Equal(input.Expected, context.Result);
            return Task.CompletedTask;
        }
    }
}
