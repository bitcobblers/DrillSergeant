using DrillSergeant.Syntax.GWT;
using static DrillSergeant.GWT;
using Shouldly;

namespace DrillSergeant.Tests.Features;

#if MSTEST
[TestClass]
#endif
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
        Given("Set context", c => c.IsSuccess = false);
        WhenAsync("Add delay", async c =>
        {
            await Task.Delay(10);
            c.IsSuccess = true;
        });
        Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
#if MSTEST
    [TestCategory("SAMPLE_CAT")]
#endif
    public void WaitUsingInlineDelay_WithContext()
    {
        Given("Set context", c => c.IsSuccess = false);
        WhenAsync<Context>("Add delay", async c =>
        {
            await Task.Delay(10);
            CurrentBehavior.Context.IsSuccess = true;
        });
        Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
    public void WaitUsingInlineDelay_WithInput()
    {
        Given("Set context", c => c.IsSuccess = false);
        WhenAsync<Input>("Add delay", async (c, i) =>
        {
            await Task.Delay(10);
            c.IsSuccess = true;
        });
        Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
    public void WaitUsingInlineDelay_WithContextAndInput()
    {
        Given("Set context", c => c.IsSuccess = false);
        WhenAsync<Context, Input>("Add delay", async (c, i) =>
        {
            await Task.Delay(10);
            CurrentBehavior.Context.IsSuccess = true;
        });
        Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
    public void WaitUsingLambdaDelay()
    {
        Given("Set context", c => c.IsSuccess = false);
        When(DelayAndSet(10));
        Then("Check result", c => ((bool)c.IsSuccess).ShouldBeTrue());
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
