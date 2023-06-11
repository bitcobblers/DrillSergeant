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
}
