using System.Threading.Tasks;
using DrillSergeant.GWT;
using Xunit;

namespace DrillSergeant.Tests.Features;

public class BaseStepFeature
{
    [Behavior]
    public void ModifyingInputFails()
    {
        var input = new
        {
            Value = "expected"
        };

        BehaviorBuilder.New(input)
            .When("Update input", (c, i) => i.Value = "error")
            .Then("Input should be unchanged", (c, i) => Assert.Equal("expected", i.Value));
    }

    [Behavior]
    public void CreatingBehaviorWithoutInputCreatesEmptyBag()
    {
        BehaviorBuilder.New()
            .Then("The input should be non-null", (c, i) => Assert.NotNull(i));
    }

    [Behavior]
    public void ReferenceTypeContextValuesArePreservedBetweenSteps()
    {
        object value = new();

        BehaviorBuilder.New()
            .Given("Set context", c => c.Value = value)
            .Then("Verify context is same", c => Assert.Same(value, c.Value));
    }

    [Behavior]
    public void ConsumingBackgroundAutomaticallyExecutesSteps()
    {
        BehaviorBuilder.New()
            .EnableContextLogging()
            .Background(SetupContext)
            .Then("Check A", c => Assert.Equal(1, c.A))
            .But("Check B", c => Assert.Equal(2, c.B));
    }

    [Behavior]
    public void BackgroundIsAbleToAccessInput()
    {
        var input = new
        {
            Value = "expected"
        };

        BehaviorBuilder.New(input)
            .EnableContextLogging()
            .Background(SetupContextFromInput)
            .Then("Check Value", c => Assert.Equal("expected", c.Value));
    }

    [Behavior]
    public void BackgroundIsAbleToAccessInputAsync()
    {
        var input = new
        {
            Value = "expected"
        };

        BehaviorBuilder.New(input)
            .EnableContextLogging()
            .Background(SetupContextFromInputAsync)
            .Then("Check Value", c => Assert.Equal("expected", c.Value));
    }

    [Behavior]
    public void BackgroundIsAbleToAccessContext()
    {
        var input = new
        {
            Value = "expected"
        };

        BehaviorBuilder.New(input)
            .EnableContextLogging()
            .Background(SetupContext)
            .Then("Check Value", c => Assert.Equal(1, c.A));
    }

    [Behavior]
    public void CallingNullLambdaHandlerDoesNotStopExecution()
    {
        BehaviorBuilder.New()
            .Given(NullLambdaStep())
            .When("Set context value", c => c.Success = true)
            .Then("Check context", c => Assert.True(c.Success));
    }

    private LambdaStep NullLambdaStep() =>
        new("Null step");

    public Behavior SetupContext =>
        new Behavior()
            .Given("Background Step 1", c => c.A = 1)
            .And("Background Step 2", c => c.B = 2);

    public Behavior SetupContextFromInput =>
        new Behavior()
            .Given("Setup Context", (c, i) => c.Value = i.Value);

    public Behavior SetupContextFromInputAsync =>
        new Behavior()
            .GivenAsync("Setup Context", (c, i) =>
            {
                c.Value = i.Value;
                return Task.CompletedTask;
            });
}
