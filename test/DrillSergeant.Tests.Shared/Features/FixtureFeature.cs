using Shouldly;
using static DrillSergeant.GWT;

namespace DrillSergeant.Tests.Features;

#if MSTEST
[TestClass]
#endif
// ReSharper disable once UnusedType.Global
public class FixtureFeature
{
    [Behavior]
    public void CallingSyncFixtureWithDeferredResultsReturnsCorrectResult()
    {
        var a = Given("Get a", () => 1);
        var b = Given("Get b", () => 2);
        var result = When(new SyncFixture(a, b));

        Then("Check result", () => CheckResult(result, 3));
    }

    [Behavior]
    public Task CallingAsyncFixtureWithDeferredResultsReturnsCorrectResult()
    {
        var a = GivenAsync("Get a", () => Task.FromResult(1));
        var b = GivenAsync("Get b", () => Task.FromResult(2));
        var result = When(new AsyncFixture(a, b));

        ThenAsync("Check result", async () => CheckResult(await result, 3));
        return Task.CompletedTask;
    }

    private void CheckResult(int actual, int expected)
    {
        actual.ShouldBe(expected);
    }

    public class SyncFixture : StepFixture<int>
    {
        private readonly StepResult<int> _a;
        private readonly StepResult<int> _b;

        public SyncFixture(StepResult<int> a, StepResult<int> b)
            : base(nameof(SyncFixture))
        {
            _a = a;
            _b = b;
        }

        public override int Execute() => _a + _b;
    }

    public class AsyncFixture : AsyncStepFixture<int>
    {
        private readonly AsyncStepResult<int> _a;
        private readonly AsyncStepResult<int> _b;

        public AsyncFixture(AsyncStepResult<int> a, AsyncStepResult<int> b)
            : base(nameof(AsyncFixture))
        {
            _a = a;
            _b = b;
        }

        public override async Task<int> Execute() => await _a + await _b;
    }
}