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
    var behavior = new Behavior<Context,Input>(input);

    // Configure behavior here...
    
    return behavior;
}
```

Behaviors are regular test methods that are decorated with the `[Behavior]` attribute and return an instance of a `Behavior` class.  Because `[Behavior]` is built on top of `[Theory]` one or more inputs must be provided to the test.  This can be done through any `xunit` data discovery mechanism (e.g. `[InlineData]`, `[MemberData]`, etc...).

## Context and Input

`DrillSergeant` is built on top of [`xunit`](https://xunit.net/) and makes use of `[Theory]` based tests.  As a result behavior tests require both a context to hold state throughout the test and input to drive the test.  These are typically defined using the C# `record` type:

```
public class Context {};
public record Input();
```

Each step within a behavior can update its context, which is then fed into the next step.  It's recommended to use the C# `record` type for inputs and `class` for context.

### Configuring Input and Context

The only required parameter to a behavior is the `input` parameter, which must be a type of `TInput`.  Context on the other hand is optional and can be omitted.  If it is, then a new instance of `TContext` will be instantiated using its parameterless constructor.

```
var input = new Input();
var behavior1 = new Behavior<Context,Input>(input); // Creates context automatically.
var behavior2 = new Behavior<Context,Input>(input, new Context()); // Manually specify context.
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
public LambdaStep<Context,Input> MyStep =>
    new GivenLambdaStep<Context,Input>()
        .Named("My step")
        .Handle( (c,i) => {
		    // Perform some action.
    });
```

As you can see, the syntax is nearly identical to an inline step.  In fact, inline steps are actually converted to lambda steps behind the scenes.

### Class Steps

Class steps are the most flexible type of step and best used when a particular step needs to be reused between multiple features.  To create a class step, override the desired verb and fill in the step method:

```
public class MyStep<Context,Input> : GivenStep<Context,Input>
{
    public override void Given(Context context, Input input)
    {
        // Perform some action.
    }
}
```

Unlike inline and lambda steps, class steps are convention based.  By default, The `GivenStep`, `WhenStep`, and `ThenStep` provide virtual methods for convenience, but it is not required to use them.  Internally, `DrillSergeant` will pick a matching verb method with the most parameters.  For example:

```
public class MyStep<Context,Input> : GivenStep<Context,Input>
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

## Configuring the Resolver

`DrillSergeant` supports dependency injection for class steps and inline steps.  This is accomplished via the `IDependencyResolver` interface.  The default behavior when injecting parameters is to invoke their default constructor.  However this can be configured.  To override this behavior, use the `ConfigureResolver()` method and supply your own:

```
var behavior = 
    new Behavior<Context,Input>(input)
        .ConfigureResolver(() => {
            var resolver = A.Fake<IDependencyResolver();

            // Configure resolver here...

            return resolver;
        });
```
In this example, we're using the mocking library `FakeItEasy` to create a resolver that returns instances of the required dependency, but for more advanced scenarios a real DI container can be substituted in its place.

## Best Practices

### Favor Xunit Class/Collection Fixtures

Xunit already has a mechanism for handling shared data with constructors, `IClassFixture<>`, and `ICollectionFixture`.  These should be preferred by default.  More information can be found [here](https://xunit.net/docs/shared-context).

As a quick recap:
* Use constructors/private fields for dependencies that are isolated at the test level.
* Use `IClassFixture<>` for dependencies that are isolated at the class level.
* Use `ICollectionFixture<>` for global dependencies.