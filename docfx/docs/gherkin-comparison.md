# Gherkin Comparison

DrillSergeant loosely follows the Gherkin syntax for writing behavior tests. The following lists common keywords and their DrillSergeant equivalent.

### Feature

#### Equivalent: Regular class.

#### Example:

```CSharp
public class MyFeature
{
    // Define behaviors here...
}
```

### Rule

#### Equivalent: Subclass

#### Example

```CSharp
public class MyFeature
{
    public class MyRule : MyFeature
    {
        // Define behaviors.
    }
}
```

### Example (Scenario)

#### Equivalent: `[Behavior]`

#### Example:

```CSharp
[Behavior]
public void MyBehavior()
{
    Given("Step 1", () => { /* Do stuff here */ });
    // ...
}
```

### Given/When/Then Steps

#### Equivalent: Predefined steps in the `GWT` namespace.

#### Example:

```CSharp
Given("Step 1", () => { /* Given logic here */ });
When("Step 2", () => { /* When logic here */ });
Then("Step 3", () => { /* Then logic here */ });
```

### Background

#### Equivalent: Behavior builder method `Background()`.

#### Example:

```CSharp
public Behavior SetupBehavior => new Behavior();

[Behavior]
public void MyBehavior()
{
    BehaviorBuilder
        .Current
        .Background(SetupBehavior);
}
```

### Scenario Outline/Scenario Template

#### Equivalent: Any attribute compatible with `[Theory]`/`[TestCaseSource]` data.

#### Example:

```CSharp
[Behavior]
[InlineData(1,2)]
[InlineData(2,3)]
public void MyBehavior(int a, int b)
{
    var sum = Given("Step 1", () => a + b);
}
```
