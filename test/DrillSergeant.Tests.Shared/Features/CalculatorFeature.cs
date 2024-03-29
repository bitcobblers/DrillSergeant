﻿using static DrillSergeant.GWT;

#if NUNIT
using AutoFixture.NUnit3;
// ReSharper disable NUnit.AutoFixture.MissedTestOrTestFixtureAttribute
#endif

#if XUNIT
using AutoFixture.Xunit2;
#endif

namespace DrillSergeant.Tests.Features;

#if MSTEST
[TestClass]
#endif
public class CalculatorFeature
{
    // ReSharper disable once UnusedMember.Local
    private readonly Calculator _calculator = new();

    public record Input(int a, int b, int expected);

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

        ThenAsync("Check result", () => CheckResultAsync(result, expected));

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

        Then("Check result", () => CheckResult(result, expected));
    }

    private int AddNumbers_Simple(int a, int b, Calculator calculator)
    {
        return calculator.Add(a, b);
    }

    private void CheckResult(int given, int expected)
    {
        given
            .ShouldBe(expected);
    }

    private async Task CheckResultAsync(Task<int> given, int expected)
    {
        (await given).ShouldBe(expected);
    }

    // Step implemented as a normal method.
    // ReSharper disable once UnusedMember.Local
    private void SetSecondNumber(int b) => CurrentBehavior.Context.b = b;

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
