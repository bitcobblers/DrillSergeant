using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class CurrentBehaviorTests
{
    public class AccessTests : CurrentBehaviorTests
    {
        [Fact]
        public Task AccessContextOutsideOfBehaviorThrowsNoCurrentBehaviorException()
        {
            // Arrange.
            CurrentBehavior.Clear();

            // Assert.
            Assert.Throws<NoCurrentBehaviorException>(() =>
            {
                _ = CurrentBehavior.Context;
            });

            return Task.CompletedTask;
        }

        [Fact]
        public Task AccessInputOutsideOfBehaviorThrowsNoCurrentBehaviorException()
        {
            // Arrange.
            CurrentBehavior.Clear();

            // Assert.
            Assert.Throws<NoCurrentBehaviorException>(() =>
            {
                _ = CurrentBehavior.Input;
            });

            return Task.CompletedTask;
        }

        [Fact]
        public Task AccessMapContextOutsideOfBehaviorThrowsNoCurrentBehaviorException()
        {
            // Arrange.
            CurrentBehavior.Clear();

            // Assert.
            Assert.Throws<NoCurrentBehaviorException>(() =>
            {
                _ = CurrentBehavior.MapContext<object>();
            });

            return Task.CompletedTask;
        }

        [Fact]
        public Task AccessMapInputOutsideOfBehaviorThrowsNoCurrentBehaviorException()
        {
            // Arrange.
            CurrentBehavior.Clear();

            // Assert.
            Assert.Throws<NoCurrentBehaviorException>(() =>
            {
                _ = CurrentBehavior.MapInput<object>();
            });

            return Task.CompletedTask;
        }
    }

    [Fact]
    public Task MappingContextTwiceThrowsContextAlreadyMappedException()
    {
        // Arrange.
        CurrentBehavior.Set(new Behavior());
        _ = CurrentBehavior.MapContext<object>();

        // Assert.
        Assert.Throws<ContextAlreadyMappedException>(() =>
        {
            _ = CurrentBehavior.MapContext<object>();
        });

        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdatingReadonlyContextDoesNotSaveChanges()
    {
        // Arrange.
        CurrentBehavior.Set(new Behavior());

        var rawContext = (IDictionary<string, object?>)CurrentBehavior.Context;
        var mappedContext = CurrentBehavior.MapContext<StubContext>(isReadonly: true);

        // Act.
        mappedContext.StringValue = "error";
        CurrentBehavior.UpdateContext();

        // Assert.
        rawContext.ShouldBeEmpty();

        return Task.CompletedTask;
    }

    [Fact]
    public Task UpdatingContextSavesChanges()
    {
        // Arrange.
        CurrentBehavior.Set(new Behavior());

        var rawContext = CurrentBehavior.Context;
        var mappedContext = CurrentBehavior.MapContext<StubContext>();

        // Act.
        mappedContext.StringValue = "expected";
        CurrentBehavior.UpdateContext();

        // Assert.
        Assert.Equal("expected", rawContext.StringValue);

        return Task.CompletedTask;
    }

    private class StubContext
    {
        public string? StringValue { get; set; }
    }
}