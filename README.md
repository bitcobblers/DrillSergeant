# DrillSergeant
.net behavior driven testing written by developers, for developers.

# Summary

`DrillSergeant` is a behavior testing library that empowers developers to apply BDD practices with minimal amount of friction.  Simply import the package and write your behaviors in familiar C# syntax.

# Getting Started

For a complete example of a feature, see the following [example](https://github.com/bitcobblers/DrillSergeant/blob/main/test/DrillSergeant.Tests/Features/CalculatorFeature.cs).

## Creating a Behavior

Creating a behavior is very simple:

```
[Behavior, InputData(1,1)]
public Behavior MyBehaviorTest(int value1, int value2)
{
    var input = new Input(value1, value2);
    var behavior = new Behavior<Input>(input);

    // Configure behavior here...
    
    return behavior;
}
```

Behaviors are regular test methods that are decorated with the `[Behavior]` attribute and return an instance of a `Behavior` class.  Because `[Behavior]` is built on top of `[Theory]` one or more inputs must be provided to the test.  This can be done through any `xunit` data discovery mechanism (e.g. `[InlineData]`, `[MemberData]`, etc...).

## Context and Input

`DrillSergeant` is built on top of [`xunit`](https://xunit.net/) and makes use of `[Theory]` based tests.  `DrillSergeant` will automatically create a local context for the behavior to execute in.  Context on the other hand must be defined and provited to the behavior.  These are typically defined using the C# `record` type:

```
public record Input();
```

Each step within a behavior can update its context, which is then fed into the next step.  While not required, it's recommended to use the C# `record` type for input.

### Configuring Input

The only required parameter to a behavior is the `input` parameter, which must be a type of `TInput`.  Context on the other hand is automatically created and maintained by `DrillSergeant`.

```
var input = new Input();
var behavior = new Behavior<Input>(input);
```

## Configuring Steps

Individual steps can be configured depending on the level of granularity required.

### Inline Steps

Inline steps are the simplest type of step.  An inline step can be added simply by calling `Given()`/`When()`/`Then()` and passing in a lambda:

```
Given("My step", (c,i) => {
    // Perform some action
});
```

Inline steps are convenient when you need a one-off step that won't be reused in other behaviors.

### Lambda Steps

Lambda steps are ideal for situations where a step needs to be reused for multiple behaviors within a single class:

```
public LambdaStep<Input> MyStep =>
    new GivenLambdaStep<Input>()
        .Named("My step")
        .Handle( (c,i) => {
            // Perform some action.
    });
```

As you can see, the syntax is nearly identical to an inline step.  In fact, inline steps are actually converted to lambda steps behind the scenes.

### Class Steps

Class steps are the most flexible type of step and best used when a particular step needs to be reused between multiple features.  To create a class step, override the desired verb and fill in the step method:

```
public class MyStep<Input> : GivenStep<Input>
{
    public override void Given(Context context, Input input)
    {
        // Perform some action.
    }
}
```

Unlike inline and lambda steps, class steps are convention based.  By default.  For example:

```
public class MyStep<Input> : GivenStep<Input>
{
    // DrillSergeant will *not* excute this.
    public override void Given(Context context, Input input)
    {
        // Perform some action.
    }
  
    // DrillSergeant will execute this.
    public override void Given(Context context, Input input, MyDependency dependency)
    {
        // Perform some action.
    }
}
```