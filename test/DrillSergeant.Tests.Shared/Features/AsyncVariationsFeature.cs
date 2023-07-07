using DrillSergeant.GWT;
using Shouldly;

namespace DrillSergeant.Tests.Features;

public class AsyncVariationsFeature
{
    public class Context
    {
        public bool IsSuccess { get; set; }
    }

    public class Input { }

    [Behavior]
    public void WaitUsingInlineDelay()
    {
        BehaviorBuilder.New()
            .Given("Set context", c => c.IsSuccess = false)
            .WhenAsync("Add delay", async c =>
            {
                await Task.Delay(10);
                c.IsSuccess = true;
            })
            .Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
    public void WaitUsingInlineDelay_WithContext()
    {
        BehaviorBuilder.New()
            .Given("Set context", c => c.IsSuccess = false)
            .WhenAsync<Context>("Add delay", async c =>
            {
                await Task.Delay(10);
                c.IsSuccess = true;
            })
            .Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
    public void WaitUsingInlineDelay_WithInput()
    {
        BehaviorBuilder.New()
            .Given("Set context", c => c.IsSuccess = false)
            .WhenAsync<Input>("Add delay", async (c, i) =>
            {
                await Task.Delay(10);
                c.IsSuccess = true;
            })
            .Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
    public void WaitUsingInlineDelay_WithContextAndInput()
    {
        BehaviorBuilder.New()
            .Given("Set context", c => c.IsSuccess = false)
            .WhenAsync<Context, Input>("Add delay", async (c, i) =>
            {
                await Task.Delay(10);
                c.IsSuccess = true;
            })
            .Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
    public void WaitUsingLambdaDelay()
    {
        BehaviorBuilder.New()
            .Given("Set context", c => c.IsSuccess = false)
            .When(DelayAndSet(10))
            .Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    public LambdaStep DelayAndSet(int milliseconds) =>
        new WhenLambdaStep()
            .SetName($"Adding delay of {milliseconds:N0}ms")
            .HandleAsync(async c =>
            {
                await Task.Delay(milliseconds);
                c.IsSuccess = true;
            });

}
