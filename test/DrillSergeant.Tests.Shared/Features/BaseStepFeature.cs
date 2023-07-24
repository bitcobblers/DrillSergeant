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

        BehaviorBuilder.Current.SetInput(input);
        When("Update input", () => CurrentBehavior.Input.Value = "error");
        Then("Input should be unchanged", () => ((string)CurrentBehavior.Input.Value).ShouldBe("expected"));
    }

    [Behavior]
    public void CreatingBehaviorWithoutInputCreatesEmptyBag()
    {
        Then("The input should be non-null", () => ((object?)CurrentBehavior.Input).ShouldNotBeNull());
    }

    [Behavior]
    public void ReferenceTypeContextValuesArePreservedBetweenSteps()
    {
        object value = new();

        Given("Set context", () => CurrentBehavior.Context.Value = value);
        Then("Verify context is same", () => ((object)CurrentBehavior.Context.Value).ShouldBeSameAs(value));
    }

    [Behavior]
    public void ConsumingBackgroundAutomaticallyExecutesSteps()
    {
        BehaviorBuilder.Reset()
            .EnableContextLogging()
            .Background(SetupContext);

        Then("Check A", () => ((int)CurrentBehavior.Context.A).ShouldBe(1));
        But("Check B", () => ((int)CurrentBehavior.Context.B).ShouldBe(2));
    }

    [Behavior]
    public void BackgroundIsAbleToAccessInput()
    {
        var input = new
        {
            Value = "expected"
        };

        BehaviorBuilder.Reset(input)
            .EnableContextLogging()
            .Background(SetupContextFromInput);

        Then("Check Value", () => ((string)CurrentBehavior.Context.Value).ShouldBe("expected"));
    }

    [Behavior]
    public void BackgroundIsAbleToAccessInputAsync()
    {
        var input = new
        {
            Value = "expected"
        };

        BehaviorBuilder.Reset(input)
            .EnableContextLogging()
            .Background(SetupContextFromInputAsync);

        Then("Check Value", () => ((string)CurrentBehavior.Context.Value).ShouldBe("expected"));
    }

    [Behavior]
    public void BackgroundIsAbleToAccessContext()
    {
        var input = new
        {
            Value = "expected"
        };

        BehaviorBuilder.Reset(input)
            .EnableContextLogging()
            .Background(SetupContext);

        Then("Check Value", () => ((int)CurrentBehavior.Context.A).ShouldBe(1));
    }

    [Behavior]
    public void CallingNullLambdaHandlerDoesNotStopExecution()
    {
        Given(NullLambdaStep());
        When("Set context value", () => CurrentBehavior.Context.Success = true);
        Then("Check context", () => ((bool)CurrentBehavior.Context.Success).ShouldBeTrue());
    }

    [Behavior]
    public void SkippingStepPreventsItFromExecuting()
    {
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
            .Given("Background Step 1", () => CurrentBehavior.Context.A = 1)
            .And("Background Step 2", () => CurrentBehavior.Context.B = 2);

    public Behavior SetupContextFromInput =>
        new Behavior()
            .Given("Setup Context", () =>
            {
                CurrentBehavior.Context.Value = CurrentBehavior.Input.Value;
            });

    public Behavior SetupContextFromInputAsync =>
        new Behavior()
            .GivenAsync("Setup Context", () =>
            {
                CurrentBehavior.Context.Value = CurrentBehavior.Input.Value;
                return Task.CompletedTask;
            });
}
