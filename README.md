# DrillSergeant
.net behavior driven testing written by developers, for developers.

# Summary

`DrillSergeant` is a behavior testing library that empowers developers to apply BDD practices with minimal amount of friction.  Simply import the package and write your behaviors in familiar C# syntax.

# Getting Started

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

TODO

### Inline Steps

TODO

### Lambda Steps

TODO

### Class Steps

TODO

## Configuring the Resolver

TODO
