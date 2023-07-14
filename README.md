# DrillSergeant
.net behavior driven testing written by developers, for developers.

[![Build](https://github.com/BitCobblers/DrillSergeant/actions/workflows/test.yml/badge.svg)](https://github.com/bitcobblers/DrillSergeant/actions/workflows/test.yml)
[![Nuget](https://img.shields.io/nuget/v/DrillSergeant.svg)](https://www.nuget.org/packages/DrillSergeant/)
[![Nuget](https://img.shields.io/nuget/dt/DrillSergeant)](https://www.nuget.org/packages/DrillSergeant/)
[![codecov](https://codecov.io/gh/bitcobblers/DrillSergeant/branch/main/graph/badge.svg?token=R9MKC6IJXE)](https://codecov.io/gh/bitcobblers/DrillSergeant)
[![CodeQL](https://github.com/bitcobblers/DrillSergeant/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/bitcobblers/DrillSergeant/actions/workflows/github-code-scanning/codeql)

# Introduction

DrillSergeant is a behavior testing library that empowers developers to apply BDD practices with minimal amount of friction.  Simply import the package and write your behaviors in familiar C# syntax.  Unlike other behavior testing frameworks which rely on external feature files to write scenarios, DrillSergeant lets you write behavior tests 100% in C# code.

## Getting Started

For a complete example of a feature, see the following [example](https://github.com/bitcobblers/DrillSergeant/blob/main/test/DrillSergeant.Tests.Shared/Features/CalculatorFeature.cs).
For something more complex, see the [DemoStore](https://github.com/bitcobblers/StoreDemo) repo.

## A Basic Calculator Service

Lets say we have a `Calculator` service.  For this example we'll define it as a simple class.
```CSharp
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Sub(int a, int b) => a - b;
}
```
We can write a behavior test like so:
```CSharp
public class CalculatorTests
{
    private readonly Calculator _calculator = new();

    [Behavior]
    [InlineData(1,2,3)] // [TestCase] for NUnit or [DataRow] for MSTest.
    [InlineData(2,3,5)]
    public void TestAdditionBehavior(int a, int b, int expected)
    {
        var input = new
        {
            A = a,
            B = b,
            Expected = expected
        };

        BehaviorBuilder.New(input)
            .Given(SetFirstNumber)
            .Given(SetSecondNumber)
            .When(AddNumbers)
            .Then(CheckResult);
    }

    [Behavior]
    [InlineData(3,2,1)]
    [InlineData(5,2,3)]
    public void TestSubtractionBehavior(int a, int b, int expected)
    {
        var input = new
        {
            A = a,
            B = b,
            Expected = expected
        };

        BehaviorBuilder.New(input)
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

Behaviors are written in same fashion as a normal unit test.  The only difference is that it is marked using the `[Behavior]` attribute.

## Why Write Tests This Way?

Unlike in normal unit tests, which are intended to test the correctness of individual methods, behaviors tests validate whether one or more components actually behave in the way expected when given "normal" inputs.  Because of this, behaviors are composed of a series of pluggable steps that can be re-used in different scenarios.  See the [Cucumber](https://cucumber.io/docs/guides/overview/) documentation for an introduction into behavior testing.

## Why Not Use A 3rd Party Acceptance Testing Tool (e.g. SpecFlow, Fitnesse, Guage)?

DrillSergeant was borne out of frustration of using 3rd party testing tools.  While tools such as SpecFlow and Guage have gotten easier to use over time, they require installing 3rd party plugins/runners in the developer environment.  Additionally they require separate files for authoring the tests themselves (`.feature` for Specflow, '.wiki' for FitNesse, and `.md` for Gauge).  This relies on a mixture of code generation and reflection magic in order to bind the test specifications with the code that actually runs them, which adds a layer of complexity.

DrillSergeant takes a different approach to this problem.  Rather than rely on DSLs and complex translation layers, it engrafts additional capabilities to the xunit framework to make it easy to write behavior-driven with familiar C# syntax.  No new DSLs to learn, no build task fussiness, no reflection shenanigans.  Just a simple API written entirely in C# code that can be tested/debugged the exact same way as all of your other unit tests.

For a longer-winded explanation, see the following [blog post](https://www.bitcobblers.com/b/behavior-driven-testing/).

## Support 

|Framework|Support|Major Version|Notes               |
|---------|-------|-------------|--------------------|
|Xunit    |Yes    |2            |Full support        |
|NUnit    |Yes    |3            |Mostly supported    |
|MSTest   |Yes    |2            |Experimental support|

Originally DrillSergeant was built around xunit and has been well tested with it.  As of version 0.2.0 support has been added for NUnit and MSTest.  

The NUnit integration is likely to be fairly stable since the framework was designed with extensibility support in mind.  This made adding hooks for DrillSergeant fairly trivial.

The MSTest integration on the other hand should be considered experimental.  This is because that framework has very limited support for extensibility and needed several somewhat invasive hacks to get working.  If anyone has experience with MSTest and would like to help with this please let us know!

## Installation

DrillSergeant is a regular library and can be installed via package manager with either the `Install-Package` or `dotnet add package` commands.  Note that because DrillSergeant is still in beta that you will need check the 'Include Prelease' checkbox to find it in nuget manager.

|Framework|Package             |Example                                                        |
|---------|--------------------|---------------------------------------------------------------|
|Xunit    |DrillSergeant.Xunit2|`dotnet add package DrillSergeant.Xunit2 --version 0.3.0-beta` |
|NUnit    |DrillSergeant.NUnit3|`dotnet add package DrillSergeant.NUnit3 --version 0.3.0-beta` |
|MSTest   |DrillSergeant.MSTest|`dotnet add package DrillSergeant.MSTest --version 0.3.0-beta` |

## More Information

For more information, please see the [wikis](https://github.com/bitcobblers/DrillSergeant/wiki).  
For an introduction, please see this [Medium](https://medium.com/@michael.vastarelli/behavior-testing-with-drill-sergeant-cd9e747688da) article.
