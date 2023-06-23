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
            .WhenAsync(async c =>
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
            .WhenAsync<Context>(async c =>
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
            .WhenAsync<Input>(async (c, i) =>
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
            .WhenAsync<Context, Input>(async (c, i) =>
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
            .Handle(async c =>
            {
                await Task.Delay(milliseconds);
                c.IsSuccess = true;
            });

}
