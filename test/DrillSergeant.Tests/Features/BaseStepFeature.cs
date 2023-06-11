using DrillSergeant.GWT;
using Xunit;

namespace DrillSergeant.Tests.Features;

public class BaseStepFeature
{
    [Behavior]
    public Behavior ModifyingInputFails()
    {
        var input = new
        {
            Value = "expected"
        };

        return new Behavior(input)
            .When("Update input", (c, i) => i.Value = "error")
            .Then("Input should be unchanged", (c, i) => Assert.Equal("expected", i.Value));
    }

    [Behavior]
    public Behavior CreatingBehaviorWithoutInputCreatesEmptyBag()
    {
        return new Behavior()
            .Then("The input should be non-null", (c, i) => Assert.NotNull(i));
    }

    [Behavior]
    public Behavior ConsumingBackroundAutomaticallyExecutesSteps()
    {
        return new Behavior()
            .EnableContextLogging()
            .Background(SetupContext)
            .Then("Check A", c => Assert.Equal(1, c.A))
            .But("Check B", c => Assert.Equal(2, c.B));
    }

    [Behavior]
    public Behavior BackgroundIsAbleToAccessInput()
    {
        var input = new
        {
            Value = "expected"
        };

        return new Behavior(input)
            .EnableContextLogging()
            .Background(SetupContextFromInput)
            .Then("Check Value", c => Assert.Equal("expected", c.Value));
    }

    [Behavior]
    public Behavior ReferenceTypeContextValuesArePreservedBetweenSteps()
    {
        object value = new();

        return new Behavior()
            .Given("Set context", c => c.Value = value)
            .Then("Verify context is same", c => Assert.Same(value, c.Value));
    }

    public Behavior SetupContext =>
        new Behavior()
            .Given("Background Step 1", c => c.A = 1)
            .And("Background Step 2", c => c.B = 2);

    public Behavior SetupContextFromInput =>
        new Behavior()
            .Given("Setup Context", (c, i) => c.Value = i.Value);
}
