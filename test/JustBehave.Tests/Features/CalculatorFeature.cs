using JustBehave.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace JustBehave.Tests.Features;

public class CalculatorBehaviors
{
    public ITestOutputHelper output;

    public record Context(int A, int B, int Result);
    public record struct Input(int A, int B, int Expected);

    public CalculatorBehaviors(ITestOutputHelper output)
    {
        this.output = output;
    }

    public static IEnumerable<object[]> AdditionInputs
    {
        get => new[]
        {
            new object[] { 1, 2, 3 },
            new object[] { 2, 3, 5 }
        };
    }

    [Behavior, MemberData(nameof(AdditionInputs))]
    public Behavior AdditionBehavior(int a, int b, int expected, [Inject] Calculator calculator)
    {
        this.output.WriteLine("Invoking AdditionBehavior()");

        return new Behavior<Context, Input>()
            .WithInput(() => new Input(a, b, expected))
            .WithContext(() => new Context(0, 0, 0))
            .Given("Set first number", (c, i) => c with { A = i.A }) // Inline step declaration.
            .Given(SetSecondNumber)
            //.When(AddNumbers(calculator))
            //.When("Add the numbers", (c, i) => c with { Result = c.A + c.B })
            .When(AddNumbers(calculator))
            .Then<CheckResultStep>();
    }

    // Step implemented as a normal method.
    public Context SetSecondNumber(Context context, Input input) => context with { B = input.B };

    // Step implemented as a lambda step for greater flexibility.
    public LambdaStep<Context, Input> AddNumbers(Calculator calculator) =>
        new LambdaWhenStep<Context, Input>()
            .Named("Add numbers")
            .Handle((c, _) => c with { Result = calculator.Add(c.A, c.B) });

    // Step implemented as type for full customization and reusability.
    public class CheckResultStep : ThenStep<Context, Input>
    {

        public override Task ThenAsync(Context context, Input input)
        {
            Assert.Equal(input.Expected, context.Result);
            return Task.CompletedTask;
        }
    }
}
