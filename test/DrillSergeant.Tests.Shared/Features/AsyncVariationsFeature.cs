using Shouldly;
using static DrillSergeant.GWT;

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
        Given("Set context", () => CurrentBehavior.Context.IsSuccess = false); //  c => c.IsSuccess = false);
        WhenAsync("Add delay", async () =>
        {
            await Task.Delay(10);
            //c.IsSuccess = true;
            CurrentBehavior.Context.IsSuccess = true;
        });
        Then("Check result", () => ((bool)CurrentBehavior.Context.IsSuccess).ShouldBeTrue());  //c => ((bool)c.IsSuccess).ShouldBeTrue());
    }

    [Behavior]
#if MSTEST
    [TestCategory("SAMPLE_CAT")]
#endif
    public void WaitUsingInlineDelay_WithContext()
    {
        Given("Set context", () => CurrentBehavior.Context.IsSuccess = false);  // c => c.IsSuccess = false);
        WhenAsync("Add delay", async () =>
        {
            var context = CurrentBehavior.MapContext<Context>();
            await Task.Delay(10);

            context.IsSuccess = true;
        });
        Then("Check result", () => ((bool)CurrentBehavior.Context.IsSuccess).ShouldBeTrue());
    }
    
    [Behavior]
    public void WaitUsingLambdaDelay()
    {
        Given("Set context", () => CurrentBehavior.Context.IsSuccess = false);
        When(DelayAndSet(10));
        Then("Check result", () => ((bool)CurrentBehavior.Context.IsSuccess).ShouldBeTrue());
    }

    public LambdaStep DelayAndSet(int milliseconds) =>
        new LambdaStep()
            .SetName($"Adding delay of {milliseconds:N0}ms")
            .HandleAsync(async () =>
            {
                await Task.Delay(milliseconds);
                CurrentBehavior.Context.IsSuccess = true;
            });
}
