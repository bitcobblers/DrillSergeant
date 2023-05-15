# DrillSergeant
.net behavior driven testing written by developers, for developers.

# Summary

`DrillSergeant` is a behavior testing library that empowers developers to apply BDD practices with minimal amount of friction.  Simply import the package and write your behaviors in familiar C# syntax.

# Getting Started

For a complete example of a feature, see [this](https://github.com/bitcobblers/DrillSergeant/blob/main/test/DrillSergeant.Tests/Features/CalculatorFeature.cs)

## Creating a Behavior

Creating a behavior is very simple:

```
[Behavior, Theory, InputData(1,1)]
public Behavior MyBehaviorTest(int value1, int value2)
{
    var behavior = new Behavior<Context,Input>()
    // Configure behavior here...
    
    return behavior;
}
```

Behaviors are regular test methods that are decorated with the `[Behavior]` attribute and return an instance of a `Behavior` class.  Because `[Behavior]` is built on top of `[Theory]` one or more inputs must be provided to the test.  This can be done through any `xunit` data discovery mechanism (e.g. `[InlineData]`, `[MemberData]`, etc...).

## Context and Input

`DrillSergeant` is built on top of [`xunit`](https://xunit.net/) and makes use of `[Theory]` based tests.  As a result behavior tests require both a context to hold state throughout the test and input to drive the test.  These are typically defined using the C# `record` type:

```
public record Context();
public record Input();
```

Each step within a behavior returns an updated context, which is then fed into the next step.  Input on the other hand is immutable and does not change between steps.  While it's not a requirement to use the `record` type, it is preferred because they're immutable by default and have syntax that makes copying them easy:

```
return context with { UpdatedField = "new_value" };
```

### Configuring Input and Context

Use the `WithInput()` and `WithContext()` methods to configure the behavior:

```
var behavior = new Behavior<Context,Input>()
    .WithInput(() => new Input())
    .WithContext(() => new Context());
```

The `WithInput()` method is used to map the arguments passed to the test method to the `Input` type used by the test.  Likewise, the `WithContext()` method is used to establish the initial context for the first step.

## Configuring Steps

Individual steps can be configured dependeing on the level of granularity required.

### Inline Steps

Inline steps are the simplest type of step.  An inline step can be added simply by calling `Given()`/`When()`/`Then()` and passing in a lambda:

```
Given("My step", (c,i) => {
    // Perform some action
    return c with { /* changes */ };
});
```

All steps pass the `context` and `input` as the first two parameters.  To pass additional dependencies you can call one of the generic overrides:

```
Given<MyDependency>("My step", (c,i, dep) => {
    // Perform some action
    return c with { /* changes */ };
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
            return c with { /* changes */ };
    });
```

As you can see, the syntax is nearly identical to an inline step.

### Class Steps

Class steps are the most flexible type of step and best used when a particular step needs to be reused between multiple features.  To create a class step, override the desired verb and fill in the step method:

```
public class MyStep<Context,Input> : GivenStep<Context,Input>
{
    public override Context Given(Context context, Input input)
    {
        // Perform some action.
        return context with { /* changes */ };
    }
}
```

Unlike inline and lambda steps, class steps are convention based.  By default, The `GivenStep`, `WhenStep`, and `ThenStep` provide virtual methods for convenience, but it is not required to use them.  Internally, `DrillSergeant` will pick a matching verb method with the most parameters.  For example:

```
public class MyStep<Context,Input> : GivenStep<Context,Input>
{
    // DrillSergeant will not excute this.
    public override Context Given(Context context, Input input)
    {
        // Perform some action.
        return context with { /* changes */ };
    }
  
    // DrillSergeant will execute this.
    public override Context Given(Context context, Input input, MyDependency dependency)
    {
        // Perform some action.
        return context with { /* changes */ };
    }
}
```

## Configuring the Resolver

`DrillSergeant` has first-class support for dependency injection.  When writing a behavior method, simply prefix any dependency with the `[Inject]` attribute:

```
public Behavior MyBehavior([Inject] MyDependency dependency)
{
    // ...
}
```

The `[Inject]` parameter is needed so that DrillSergeant can differentiate between input parameters passed by `[Theory]` and what it needs to inject.

Dependency resolution is handled with the `IDependencyResolver` interface, which contains a single method: `object Resolve(Type type)`.  By default `DrillSergeant` will satisfy dependencies by instantiating new instances of them via the `Activator.CreateInstance()` method.  To override this: a custom resolver can be configured in the behavior class:

```
[BehaviorResolverSetup]
public IDependencyResolver SetupResolver()
{
    var resolver = A.Fake<IDependencyResolver>();
  
    A.CallTo(() => resolver.Resove(typeof(MyDependency))).Returns(new MyDependency);
}
```

In this example, we're using the mocking library `FakeItEasy` to create a resolver that returns instances of the required dependency, but for more advanced scenarios a real DI container can be substituted in its place.

**Note: The resolver is scoped to each test case and does not share data between tests.  To share data, use xunit's `ClassFixture` and `CollectionFixture` fixtures.**
**Note: The name of the method here is unimportant.  `DrillSergeant` will look for the first `public` method returning an `IDependencyResolver` that is marked with the `[BehaviorSetupResolver]` attribute.c 

## Best Practices

### No Logic in Behaviors

Internally `DrillSergeant` will execute the behavior method prior to executing any test cases, therefore it's important not to write any logic within the behavior itself.  The behavior should only be a single a single return statement.

```
public Behavior MyBehavior([Inject] MyDependency dependency)
{
    dependency.Initialize(); // Don't do this!!!
    
    return new Behavior<Context,Input>();
}
```

If you need to perform initialization on dependencies prior to running the test, they should be performed within the `SetupResolver()` method.

### Don't Mutate Context.  Instead Return New Context

Future editions of `DrillSergeant` will keep a running history of context as it executes behavior.  Therefore it's a good idea to make sure that when returning from a step that a new context is created based on the previous step.  This can be accomplished easily using the `with` construct.

### Don't Mutate Input.  Just Don't.

The input for each behavior scenario is tied to the test results in xunit.  Once the input has been mapped with the `WithInput()` method, do not modify it again.

### Don't Map Input or Context More Than Once

The `WithInput()` and `WithContext()` methods are only executed once prior to executing any steps.  Calling either method multiple times with only result in the previous handler being overwritten.
