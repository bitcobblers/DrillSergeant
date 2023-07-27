using DrillSergeant.Syntax.GWT;
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

        Given("Set first number", (c, i) => c.a = i.a); // Inline step declaration.
        And<Input>(SetSecondNumber);
        When(AddNumbers(_calculator));
        Then<CheckResultStep>();
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
    public void AsyncAdditionBehavior(int a, int b, int expected)
    {
        Given("Set first number", (c, i) => c.a = i.a);
        GivenAsync(SetSecondNumberAsync);
        When(AddNumbersAsync(_calculator));
        Then<CheckResultStepAsync>();
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

        var calculator = Given_Ex("Create calculator", () => new Calculator());
        var result = When_Ex("Add numbers", () => AddNumbers_Simple(a, b, calculator));

        Then_Ex("Check result", () => result.Resolve().ShouldBe(expected));
    }

    private int AddNumbers_Simple(int a, int b, StepResult<Calculator> calculator)
    {
        return calculator.Resolve().Add(a, b);
    }


    // Step implemented as a normal method.
    private void SetSecondNumber(dynamic context, Input input) => context.b = input.b;

    private Task SetSecondNumberAsync(dynamic context, dynamic input)
    {
        context.b = input.b;
        return Task.CompletedTask;
    }

    // Step implemented as a lambda step for greater flexibility.
    public LambdaStep AddNumbers(Calculator calculator) =>
        new LambdaStep("Add numbers")
            .Handle((c) =>
            {
                c.Result = calculator.Add(c.a, c.b);
            });

    public LambdaStep AddNumbersAsync(Calculator calculator) =>
        new LambdaStep("Add numbers")
            .HandleAsync((c, _) =>
            {
                c.Result = calculator.Add(c.a, c.b);
                return Task.CompletedTask;
            });

    // Step implemented as type for full customization and reusability.
    public class CheckResultStep : ThenStep
    {
        public void Then(dynamic context, Input input)
        {
            int expected = input.expected;
            int result = context.Result;

            expected.ShouldBe(result);
        }
    }

    // Class-level step.
    public class CheckResultStepAsync : ThenStep
    {
        public Task ThenAsync(dynamic context, Input input)
        {
            int expected = input.expected;
            int result = context.Result;

            expected.ShouldBe(result);
            return Task.CompletedTask;
        }
    }

    public class Calculator
    {
        public int Add(int a, int b) => a + b;
    }
}
