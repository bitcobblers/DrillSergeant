using DrillSergeant.Syntax.GWT;
using static DrillSergeant.GWT;
using Shouldly;

namespace DrillSergeant.Tests.Features;

#if MSTEST
[TestClass]
#endif
public class BaseStepFeature
{
    [Behavior]
    public void ModifyingInputFails()
    {
        var input = new
        {
            Value = "expected"
        };

        BehaviorBuilder.New(input);
        When("Update input", (c, i) => i.Value = "error");
        Then("Input should be unchanged", (_, i) => ((string)i.Value).ShouldBe("expected"));
    }

    [Behavior]
    public void CreatingBehaviorWithoutInputCreatesEmptyBag()
    {
        BehaviorBuilder.New();
        Then("The input should be non-null", (_, i) => ((object?)i).ShouldNotBeNull());
    }

    [Behavior]
    public void ReferenceTypeContextValuesArePreservedBetweenSteps()
    {
        object value = new();

        BehaviorBuilder.New();
        Given("Set context", c => c.Value = value);
        Then("Verify context is same", c => ((object)c.Value).ShouldBeSameAs(value));
    }

    [Behavior]
    public void ConsumingBackgroundAutomaticallyExecutesSteps()
    {
        BehaviorBuilder.New()
            .EnableContextLogging()
            .Background(SetupContext);

        Then("Check A", c => ((int)c.A).ShouldBe(1));
        But("Check B", c => ((int)c.B).ShouldBe(2));
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
            .Background(SetupContextFromInput);

        Then("Check Value", c => ((string)c.Value).ShouldBe("expected"));
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
            .Background(SetupContextFromInputAsync);

        Then("Check Value", c => ((string)c.Value).ShouldBe("expected"));
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
            .Background(SetupContext);

        Then("Check Value", c => ((int)c.A).ShouldBe(1));
    }

    [Behavior]
    public void CallingNullLambdaHandlerDoesNotStopExecution()
    {
        BehaviorBuilder.New();

        Given(NullLambdaStep());
        When("Set context value", c => c.Success = true);
        Then("Check context", c => ((bool)c.Success).ShouldBeTrue());
    }

    [Behavior]
    public void SkippingStepPreventsItFromExecuting()
    {
        BehaviorBuilder.New();

        Given(SkippedStep());
        Then("should execute", () => { });
    }

    private LambdaStep NullLambdaStep() =>
        new("Null step");

    private LambdaStep SkippedStep() =>
        new LambdaStep("Skipped step")
            .Handle(() => throw new Exception("I SHOULD NOT HAVE EXECUTED"))
            .Skip();

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
