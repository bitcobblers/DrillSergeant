using DrillSergeant.GWT;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests.Features;

public class AsyncVariationsFeature
{
    public class Context
    {
        public bool IsSuccess { get; set; }
    }

    public class Input { }

    [Behavior]
    public Behavior WaitUsingInlineDelay()
    {
        return new Behavior()
            .Given("Set context", c => c.IsSuccess = false)
            .WhenAsync("Add delay", async c =>
            {
                await Task.Delay(10);
                c.IsSuccess = true;
            })
            .Then("Check result", c => Assert.True(c.IsSuccess));
    }

    [Behavior]
    public Behavior WaitUsingInlineDelay_WithContext()
    {
        return new Behavior()
            .Given("Set context", c => c.IsSuccess = false)
            .WhenAsync<Context>("Add delay", async c =>
            {
                await Task.Delay(10);
                c.IsSuccess = true;
            })
            .Then("Check result", c => Assert.True(c.IsSuccess));
    }

    [Behavior]
    public Behavior WaitUsingInlineDelay_WithInput()
    {
        return new Behavior()
            .Given("Set context", c => c.IsSuccess = false)
            .WhenAsync<Input>("Add delay", async (c, i) =>
            {
                await Task.Delay(10);
                c.IsSuccess = true;
            })
            .Then("Check result", c => Assert.True(c.IsSuccess));
    }

    [Behavior]
    public Behavior WaitUsingInlineDelay_WithContextAndInput()
    {
        return new Behavior()
            .Given("Set context", c => c.IsSuccess = false)
            .WhenAsync<Context, Input>("Add delay", async (c, i) =>
            {
                await Task.Delay(10);
                c.IsSuccess = true;
            })
            .Then("Check result", c => Assert.True(c.IsSuccess));
    }

    [Behavior]
    public Behavior WaitUsingLambdaDelay()
    {
        return new Behavior()
            .Given("Set context", c => c.IsSuccess = false)
            .When(DelayAndSet(10))
            .Then("Check result", c => Assert.True(c.IsSuccess));
    }

    public LambdaStep DelayAndSet(int milliseconds) =>
        new LambdaWhenStep()
            .Named($"Adding delay of {milliseconds:N0}ms")
            .HandleAsync(async c =>
            {
                await Task.Delay(milliseconds);
                c.IsSuccess = true;
            });

}
