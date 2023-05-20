using DrillSergeant.GWT;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DrillSergeant.Tests.Features;

public class CalculatorBehaviors
{
    private readonly ITestOutputHelper output;
    private readonly Calculator calculator = new Calculator();

    public interface ILoggerContext
    {
        ITestOutputHelper? Logger { get; set; }
    }

    public interface IEmptyInput { }

    public class Context : ILoggerContext
    {
        public int A { get; set; }
        public int B { get; set; }
        public int Result { get; set; }

        public ITestOutputHelper? Logger { get; set; }
    }

    public record Input(int A, int B, int Expected) : IEmptyInput;

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
    public Task<Behavior<Context,Input>> AsyncAdditionBehavior(int a, int b, int expected)
    {
        var behavior = new Behavior<Context, Input>(new Input(a, b, expected))
            .Given("Set first number", (c, i) => c.A = i.A)
            .Given(SetSecondNumberAsync)
            .When(AddNumbersAsync(calculator))
            .Then(new CheckResultStepAsync());

        return Task.FromResult(behavior);
    }

    private IDependencyResolver ConfigureResolver()
    {
        var resolver = A.Fake<IDependencyResolver>();

        A.CallTo(() => resolver.Resolve(typeof(Calculator))).Returns(this.calculator);

        return resolver;
    }

    [Behavior, MemberData(nameof(AdditionInputs))]
    public IBehavior AdditionBehavior(int a, int b, int expected)
    {
        var input = new Input(a, b, expected);

        return new Behavior<Context, Input>(input)
            .EnableContextLogging()
            .ConfigureResolver(ConfigureResolver)
            .Given("Configure logging", ConfigureLogger)
            .Given("Set first number", (c, i) => c.A = i.A) // Inline step declaration.
            .Given(SetSecondNumber)
            .When(AddNumbers(calculator))
            .Then(new CheckResultStep());
    }

    // Step implemented as a normal method.
    private void SetSecondNumber(Context context, Input input) => context.B = input.B;

    private Task SetSecondNumberAsync(Context context, Input input)
    {
        context.B = input.B;
        return Task.CompletedTask;
    }

    // Step implemented as a lambda step for greater flexibility.
    public LambdaStep<Context, Input> AddNumbers(Calculator calculator) =>
        new LambdaWhenStep<Context, Input>()
            .Named("Add numbers")
            .Handle((c) =>
            {
                c.Result = calculator.Add(c.A, c.B);
            });

    public LambdaStep<Context, Input> AddNumbersAsync(Calculator calculator) =>
        new LambdaWhenStep<Context, Input>()
            .Named("Add numbers")
            .Handle((c, _) =>
            {
                c.Result = calculator.Add(c.A, c.B);
                return Task.CompletedTask;
            });

    // Step implemented as type for full customization and reusability.
    public class CheckResultStep : ThenStep<Context, Input>
    {
        public override void Then(Context context, Input input)
        {
            context.Logger?.WriteLine("Checking result");
            Assert.Equal(input.Expected, context.Result);
        }
    }

    public class CheckResultStepAsync : ThenStep<Context, Input>
    {
        public Task ThenAsync(Context context, Input input)
        {
            Assert.Equal(input.Expected, context.Result);
            return Task.CompletedTask;
        }
    }

    private void ConfigureLogger(ILoggerContext loggerContext, IEmptyInput input)
    {
        loggerContext.Logger = this.output;
    }
}
