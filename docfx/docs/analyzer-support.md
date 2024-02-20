# Analyzer Support

Version 1.1.0 of DrillSergent introduced the `DrillSergeant.Analyzers` package which contains a collection of code checks and fixes to ensure behavior tests are written following best practices.

## Installation

Using the `dotnet` CLI, you can add the analyzers package with the following command:

```
dotnet add package DrillSergeant.Analyzers
```

## Analyzers

The following analyzers are available.

| Rule ID | Category | Severity | Fix Available? | Description                                |
| ------- | -------- | -------- | -------------- | ------------------------------------------ |
| DS0001  | Design   | Warning  | Yes            | Behavior methods should be public.         |
| DS0004  | Design   | Warning  | No             | Behavior methods cannot be static.         |
| DS0005  | Usage    | Error    | No             | Input is immutable and cannot be modified. |

### DS0001 / BehaviorMethodAccessorAnalyzer

**Cause**: A behavior test method isn't marked as `public`.
**How to fix**: Behavior test methods must be marked as `public` in order to execute.
**Example**:

```CSharp
// Incorrect
[Behavior]
private void MyBehavior()  // Only public methods can be executed by the test runner.
{
    // Test steps go here...
}

// Correct
[Behavior]
public void MyBehavior()
{
    // Test steps go here...
}
```

### DS0004 / BehaviorMethodScopeAnalyzer

**Cause**: A behavior test method cannot be `static`. It can only be an instance method.
**How to fix**: Remove the `static` modifier from the method to convert it into an instance method.
**Example**:

```CSharp
// Incorrect
[Behavior]
public static void MyBehavior() // The test runner will only execute instance methods from a class
{
    // Test steps go here...
}

// Correct
[Behavior]
public void MyBehavior()
{
    // Test steps go here...
}
```

### DS0005 / MutatingInputAnalyzer

**Cause**: Attempting to modify the `input` of a test.  
**How to fix**: The input should be treated as immutable and never modified. Only the `context` is safe to modify. If you need a type-safe instance of the `input` you can call `CurrentBehavior.MapInput<T>()`. DrillSergeant has safeguards to prevent the `input` from being modified from "obvious" causes but won't stop a determined developer.
**Example**:

```CSharp
[Behavior, InlineData(1, 0, 0)]
public void TestArithmetic(int a, int b, int result)
{
    var calculator = Given("Create a calculator", () => new Calculator());

    // Don't do this!!!
    Given("Check for potential div 0 exceptions", () => {
        int b = (int)CurrentBehavior.Input.b;
        if(b == 0)
        {
            CurrentBehavior.Input.a = 1;
            CurrentBehavior.Input.b = 1;
            CurrentBehavior.Input.result = 1;
        }
    });

    var result = When("Divide numbers", () => {
        int a = (int)CurrentBehavior.Input.a;
        int b = (int)CurrentBehavior.Input.b;
        return calculator.Resolve().Divide(a, b);
    });

    Then("Compare result", Assert.Equal((int)CurrentBehavior.Input.result, result);
}
```
