namespace DrillSergeant.Tests;

public class AsyncLazyTests
{
    [Fact]
    public async Task CanGetValueFromSyncMethod()
    {
        // Arrange.
        var lazy = new AsyncLazy<bool>(() => true);

        // Act.
        var result = await lazy;
        var resultValue = await lazy.Value;

        // Assert.
        result.ShouldBeTrue();
        resultValue.ShouldBeTrue();
    }

    [Fact]
    public async Task CanGetValueFromAsyncMethod()
    {
        // Arrange.
        var lazy = new AsyncLazy<bool>(() => Task.FromResult(true));

        // Act.
        var result = await lazy;
        var resultValue = await lazy.Value;

        // Assert.
        result.ShouldBeTrue();
        resultValue.ShouldBeTrue();
    }
}
