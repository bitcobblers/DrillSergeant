# Step Results

In addition to being able to store state information within the `CurrentBehavior.Context` dictionary, it is possible to for steps to return a value. For example.

```CSharp
// Synchronous.
var service = Given("Create service", () => new MyService());

// Asynchronous.
var service = GivenAsync("Create a service", () => Task.FromResult(new MyService()));
```

Depending on the step variant (sync or async) the verb will return either a `StepResult<T>` or `AsyncStepResult<T>` which wraps the return value within a `Lazy<T>`. From here the underlying result can be retrieved by calling `Resolve()` on it.

This is useful when you want to use the result of one step as input for another step.

```CSharp
var service = Given("Create service", () => new MyService());

When("Call service", () => DoSomething(service.Resolve()));

// ...

private void DoSomething(MyService service)
{
    // Do work...
}
```

This can actually be simplified. When executing within a closure there's no need to call `Resolve()` since the type will be implicitly converted to its underlying type.

```CSharp
When("Call Service", () => DoSomething(service)); // Implicitly calls Resolve();
```

Async results work the exact same way.

```CSharp
WhenAsync("Call Service", async () => DoSomething(await serviceAsTask));
```

Keep in mind that implicit conversion only works within a closure. If you need to pass the result as a value then you will need to cache the wrapped type and call `Resolve()` on it.

```CSharp

private LambdaStep DoSomething(StepResult<MyService> service)
{
    return
        new LambdaStep("Call Service")
            .Handle( () => {
                service.Resolve();
            });
}

```

This is because steps are not evaluated until the behavior executes.

## Execution Scope

There are a couple of things to keep in mind when using step results.

### Scoping

Step results are not evaluated until the behavior runs. Any attempt to read the result of a step beforehand will result in an `EagerStepResultEvaluationException` being thrown.

### Eager Evaluation

Step results are evaluated as soon as the step is executed, not when a the `Resolve()` method is called. This is because DrillSergeant needs to ensure that the step completed without throwing an exception before moving onto the next step.

### Exactly Once Evaluation

Calling `Resolve()` on a step result will not cause the result to be evaluated a second time. Once resolved, it will return the same object every time it is called.
