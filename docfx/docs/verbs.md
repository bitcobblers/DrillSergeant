# Verbs

In behavior testing it is common to categorize steps using `Given`/`When`/`Then` (GWT) convention. For example:

```text
Given I have 99 bottles of beer on the wall
When I take 1 down and pass it around
Then I now have 98 bottles of beer on the wall
```

Internally, DrillSergeant associates each step with a verb. The core library does not assign any specific meaning to them however. They are largely only used for reporting and handler lookups. In fact, the only API calls exposed directly by `Behavior` when it comes to adding steps are `AddStep(IStep)` and `Background(Behavior)`.

To make things easier, there is the static class `StepBuilder` that can be used to add steps to the current behavior. This class contains a number of overrides that let you set the verb, name, and optionally function handler so that regular functions can be registered as a behavior.

In order to more closely align with Gherkin syntax a number of `Behavior`, DrillSergeant provides two static classes that implement common verbs for Given/When/Then and Arrange/ActAssert. They are located in the `GWT` and `AAA` classes respectively. Behind the scenes the methods in these class call `StepBuilder`. It is preferred to use these methods instead of adding steps directly to the behavior.

DrillSergeant has no opinion on the verbs themselves. Therefore new verb sets can easily be added. It is also possible to mix different verbs within a single behavior. For example:

```CSharp
Given("Step1", () => {})
StepBuilder.AddStep("MyVerb", new MyCustomStep());
Assert("Assertion Step", () => {});
```

## Handler Resolution

When executing an individual step, DrillSergeant needs to determine which method within the instance to invoke. This is done by scanning it for all methods named after the verb (optional `Async`) and picking the match without any parameters with `async` being preferred. For example:

```CSharp
public class MyGivenStep : GivenStep
{
    // Loses -- All other overloads have more parameters.
    public void Given()
    {
    }

    // Wins -- is async.
    public Task GivenAsync()
    {
        return Task.TaskCompleted;
    }
}
```

If DrillSergeant is unable to find any handlers matching the verb it will throw a `MissingVerbHandlerException`. If it finds a handler with parameters it will throw an `AmbiguousVerbHandlerException`.

## Custom Verbs

To create a custom verb, derive a class from either `VerbStep` or `LambdaStep` and pass the desired verb into the constructor.

```CSharp
public class AssertStep : VerbStep
{
    public AssertStep()
    {
    }

    public AssertStep(string? name)
        : base(name)
    {
    }

    public override string Verb => "Assert";
}

public class AssertLambdaStep : LambdaStep
{
    public AssertStep()
    {
        SetVerb("Assert");
    }

    public AssertLambdaStep(string? name)
        : base(name)
    {
        SetVerb("Assert");
    }
}
```

Note: While it's possible to `override` the default implementation for `Verb` when deriving from `LambdaStep` it's best not to since it has it's own internal logic.
