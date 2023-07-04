# DrillSergeant
.net behavior driven testing written by developers, for developers.

![Build](https://github.com/BitCobblers/DrillSergeant/actions/workflows/test.yml/badge.svg)
![Nuget](https://img.shields.io/nuget/v/DrillSergeant.svg)
[![codecov](https://codecov.io/gh/bitcobblers/DrillSergeant/branch/main/graph/badge.svg?token=R9MKC6IJXE)](https://codecov.io/gh/bitcobblers/DrillSergeant)

# Introduction

`DrillSergeant` is a behavior testing library that empowers developers to apply BDD practices with minimal amount of friction.  Simply import the package and write your behaviors in familiar C# syntax.

## Getting Started

For a complete example of a feature, see the following [example](https://github.com/bitcobblers/DrillSergeant/blob/main/test/DrillSergeant.Tests/Features/CalculatorFeature.cs).

## A Basic Calculator Service

Lets say we have a `Calculator` service.  For this example we'll define it as a simple class.
```
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Sub(int a, int b) => a - b;
}
```
We can write a behavior test like so:
```
public class CalculatorTests
{
    private readonly Calculator _calculator = new();

    [Behavior]
    [InlineData(1,2,3)]
    [InlineData(2,3,5)]
    public Behavior TestAdditionBehavior(int a, int b, int expected)
    {
        var input = new
        {
            A = a,
            B = b,
            Expected = expected
        };

        return new Behavior(input)
            .Given(SetFirstNumber)
            .Given(SetSecondNumber)
            .When(AddNumbers)
            .Then(CheckResult);
    }

    [Behavior]
    [InlineData(3,2,1)]
    [InlineData(5,2,3)]
    public Behavior TestSubtractionBehavior(int a, int b, int expected)
    {
        var input = new
        {
            A = a,
            B = b,
            Expected = expected
        };

        return new Behavior(input)
            .Given(SetFirstNumber)
            .Given(SetSecondNumber)
            .When(SubtractNumbers)
            .Then(CheckResult);
    }

    // Given Steps.
    private void SetFirstNumber(dynamic context, dynamic input) => context.A = input.A;
    private void SetSecondNumber(dynamic context, dynamic input) => context.B = input.B;

    // When Steps.
    private void AddNumbers(dynamic context) => context.Result = _calculator.Add(context.A, context.B);
    private void SubtractNumbers(dynamic context) => context.Result = _calculator.Sub(context.A, context.B);

    // Then Steps.
    private void CheckResult(dynamic context, dynamic input) => Assert.Equal(input.Expected, context.Result);
}
```

Behaviors are written in same fashion as a normal xunit `[Fact]` or `[Theory]` test.  The only difference is that it is marked using the `[Behavior]` attribute and must return an instance of type `Behavior`.

## Why Write Tests This Way?

Unlike in normal unit tests, which are intended to test the correctness of individual methods, behaviors tests validate whether one or more components actually behave in the way expected when given "normal" inputs.  Because of this, behaviors are composed of a series of pluggable steps that can be re-used in different scenarios.  See the [Cucumber](https://cucumber.io/docs/guides/overview/) documentation for an introduction into behavior testing.

## Why Not Use A 3rd Party Acceptance Testing Tool (e.g. SpecFlow, Fitnesse, Guage)?

`DrillSergeant` was borne out of frustration of using 3rd party testing tools.  While tools such as SpecFlow and Guage have gotten easier to use over time, they require installing 3rd party plugins/runners in the developer environment.  Additionally they require separate files for authoring the tests themselves (`.feature` for Specflow, and `.md` for Gauge).  This relies on a mixture of code generation and reflection magic in order to bind the test specifications with the code that actually runs them, which adds a layer of complexity.

`DrillSergeant` takes a different approach to this problem.  Rather than rely on DSLs and complex translation layers, it engrafts additional capabilities to the xunit framework to make it easy to write behavior-driven with familiar C# syntax.  No new DSLs to learn, no build-time generated C# code, no reflection shenanigans.  Just a simple API written 100% in C# code that can be tested/debugged the exact same way as all of your other unit tests.

For a longer-winded explanation, see the following [blog post](https://www.bitcobblers.com/b/behavior-driven-testing/).

## Current Limitations

Currently `DrillSergeant` is only compatible with `xunit` 2.4.x.  Support for `nunit` and `mstest` is planned for future releases.

## More Information

For more information, please see the [wikis](https://github.com/bitcobblers/DrillSergeant/wiki).
