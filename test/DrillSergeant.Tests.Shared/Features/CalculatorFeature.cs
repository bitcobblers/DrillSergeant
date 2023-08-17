using static DrillSergeant.GWT;
using Shouldly;

#if NUNIT
using AutoFixture.NUnit3;
#endif

#if XUNIT
using AutoFixture.Xunit2;
using Xunit.Abstractions;
#endif

namespace DrillSergeant.Tests.Features;

#if MSTEST
[TestClass]
#endif
public class CalculatorFeature
{
#if XUNIT
    private readonly ITestOutputHelper _outputHelper;
#endif
    private readonly Calculator _calculator = new();

    public record Input(int a, int b, int expected);


#if XUNIT
    public CalculatorFeature(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;
#endif

    public static IEnumerable<object[]> AdditionInputs
    {
        get => new[]
        {
            new object[] { 1, 2, 3 },
            new object[] { 2, 3, 5 },
            new object[] { 3, 4, 7 }
        };
    }

#if !MSTEST // AutoFixture does not support MSTest.
    [Behavior(Feature = "Calculator")]
    [InlineAutoData]
    [InlineAutoData]
    public void AdditionBehaviorWithAutoData(int a, int b)
    {
        BehaviorBuilder
            .Current
            .SetInput(new
            {
                a,
                b,
                expected = a + b
            })
            .EnableContextLogging();

        Given("Set first number", () => CurrentBehavior.Context.a = a); // Inline step declaration.
        And(() => SetSecondNumber(b));
        When(AddNumbers(_calculator));
        Then("Check result", () =>
        {
            int expected = (int)CurrentBehavior.Input.expected;
            int result = (int)CurrentBehavior.Context.Result;

            expected.ShouldBe(result);
        });
    }
#endif

    [Behavior]
#if XUNIT
    [MemberData(nameof(AdditionInputs))]
#endif
#if NUNIT
    [TestCaseSource(nameof(AdditionInputs))]
#endif
#if MSTEST
    [DynamicData(nameof(AdditionInputs))]
#endif
    public Task AsyncAdditionBehavior(int a, int b, int expected)
    {
        var calculator = Given("Create calculator", () => new Calculator());
        var result = WhenAsync("Add numbers", () => Task.FromResult(AddNumbers_Simple(a, b, calculator)));

        ThenAsync("Check result", async () => (await result.Resolve()).ShouldBe(expected));

        return Task.CompletedTask;
    }

    [Behavior]
#if XUNIT
    [MemberData(nameof(AdditionInputs))]
#endif
#if NUNIT
    [TestCaseSource(nameof(AdditionInputs))]
#endif
#if MSTEST
    [DynamicData(nameof(AdditionInputs))]
#endif
    public void AdditionBehavior(int a, int b, int expected)
    {
        BehaviorBuilder
            .Current
            .EnableContextLogging();

        var calculator = Given("Create calculator", () => new Calculator());
        var result = When("Add numbers", () => AddNumbers_Simple(a, b, calculator));

        Then("Check result", () => result.Resolve().ShouldBe(expected));
    }

    private int AddNumbers_Simple(int a, int b, StepResult<Calculator> calculator)
    {
        return calculator.Resolve().Add(a, b);
    }

    // Step implemented as a normal method.
    private void SetSecondNumber(int b) => CurrentBehavior.Context.b = b;

    private Task SetSecondNumberAsync()
    {
        CurrentBehavior.Context.b = CurrentBehavior.Input.b;
        return Task.CompletedTask;
    }

    // Step implemented as a lambda step for greater flexibility.
    public LambdaStep AddNumbers(Calculator calculator) =>
        new LambdaStep("Add numbers")
            .Handle(() =>
            {
                var context = CurrentBehavior.Context;
                context.Result = calculator.Add(context.a, context.b);
            });

    public LambdaStep AddNumbersAsync(Calculator calculator) =>
        new LambdaStep("Add numbers")
            .HandleAsync(() =>
            {
                var context = CurrentBehavior.Context;
                context.Result = calculator.Add(context.a, context.b);
                return Task.CompletedTask;
            });
    
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
    }
}
