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

All steps pass the `context` and `input` as the first two parameters.  To pass additional dependencies, can call one of the generic overrides:

```
Given<MyDependency>("My step", (c,i, dep) => {
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
        return context with { /* changes */ };
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

`DrillSergeant` has first-class support for dependency injection.  When writing a behavior method, simply prefix any dependency with the `[Inject]` attribute:

```
[Behavior]
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

### Keep Logic In Behaviors to a Minimum

Logic for behaviors should go in their respective steps.  Likewise setup code for dependency resolution should be taken care of within the [BehaviorResolverSetup] method.  Try to avoid writing any code within the behavior itself unless it is trivial.

Internally `DrillSergeant` will execute the behavior method prior to executing any test cases, therefore it's important not to write any logic within the behavior itself.  The behavior should only be a single a single return statement.

```
public Behavior MyBehavior(int a, int b, [Inject] MyDependency dependency)
{
    var input = new Input(a,b); // This is ok.
    dependency.Initialize(); // Put this in the resolver setup method.
    
    return new Behavior<Context,Input>(input);
}
```

### Favor Xunit Class/Collection Fixtures

Xunit already has a mechanism for handling injection of dependencies with `IClassFixture<>` and `ICollectionFixture`.  These should be preferred by default.  The `IDependencyResolver` is experimental and may be removed before the official release.