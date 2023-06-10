# DrillSergeant
.net behavior driven testing written by developers, for developers.

# Summary

`DrillSergeant` is a behavior testing library that empowers developers to apply BDD practices with minimal amount of friction.  Simply import the package and write your behaviors in familiar C# syntax.

# Getting Started

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
    public int TestAdditionBehavior(int a, int b, int expected)
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
    public int TestSubtractionBehavior(int a, int b, int expected)
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

Behaviors are written in same fashion as a normal xunit `[Theory]`.  The only difference is that it is marked using the `[Behavior]` attribute and must return an instance of type `IBehavior`.

### Why Write Tests This Way?

Unlike in normal unit tests, behavior tests are composed of a series of pluggable steps that can be re-used to test multiple behaviors.  See the [Cucumber](https://cucumber.io/docs/guides/overview/) documentation for an introduction into behavior testing.

### More Information

For more information, please see the wikis.
