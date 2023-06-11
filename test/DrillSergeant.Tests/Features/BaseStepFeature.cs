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
}
