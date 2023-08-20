using System.Collections.Generic;
using System.Threading.Tasks;

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

    public class UpdateContextMethod : CurrentBehaviorTests
    {
        [Fact]
        public void UpdatesContextWithNewFields()
        {
            // Arrange.
            var expected = new Dictionary<string, object?>
            {
                ["IntValue"] = 1
            };
            var context = new Dictionary<string, object?>();
            var changedContext = new StubWithValue { IntValue = 1 };

            // Act.
            CurrentBehavior.UpdateContext(context, changedContext);

            // Assert.
            context.ShouldBe(expected);
        }

        [Fact]
        public void UpdatesExistingFieldInContext()
        {
            // Arrange.
            var context = new Dictionary<string, object?>
            {
                ["IntValue"] = -1
            };

            var changedContext = new StubWithValue { IntValue = 1 };

            // Act.
            CurrentBehavior.UpdateContext(context, changedContext);

            // Assert.
            context.ShouldContainKey("IntValue");
            context["IntValue"].ShouldBe(1);
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

        public class StubWithValue
        {
            public int IntValue { get; set; }

            private int PrivateIntValue { get; set; }

            public static int StaticIntValue { get; set; }
        }
    }

    public class MapContextMethod : CurrentBehaviorTests
    {
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
    }

    public class CopyInputMethod : CurrentBehaviorTests
    {
        [Fact]
        public void EmptyInputReturnsEmptyResult()
        {
            // Arrange.
            var input = new Dictionary<string, object?>();

            // Act.
            var result = CurrentBehavior.CopyInput(input);

            // Assert.
            result.ShouldBeEmpty();
        }

        [Fact]
        public void CreatesShallowCopyOfData()
        {
            // Arrange.
            var obj = new object();
            var input = new Dictionary<string, object?>
            {
                ["key"] = obj
            };

            // Act.
            var result = CurrentBehavior.CopyInput(input);

            // Assert.
            result["key"].ShouldBeSameAs(obj);
        }
    }

    public class BehaviorStateTests : CurrentBehaviorTests
    {
        [Fact]
        public void CopiedInputContainsShallowCopyOfBehaviorInput()
        {
            // Arrange.
            var obj = new object();
            var behavior = new Behavior();

            behavior.Input.Add("key", obj);

            // Act.
            var state = new CurrentBehavior.BehaviorState(behavior);

            // Assert.
            state.CopiedInput.ShouldNotBeSameAs(behavior.Input);
            state.CopiedInput["key"].ShouldBeSameAs(behavior.Input["key"]);
        }

        [Fact]
        public void IsTrackedContextReadonlyIsFalseByDefault()
        {
            // Arrange.
            var behavior = new Behavior();

            // Act.
            var state = new CurrentBehavior.BehaviorState(behavior);

            // Assert.
            state.IsTrackedContextReadonly.ShouldBeFalse();
        }

        [Fact]
        public void TrackedContextReadonlyIsNullByDefault()
        {
            // Arrange.
            var behavior = new Behavior();

            // Act.
            var state = new CurrentBehavior.BehaviorState(behavior);

            // Assert.
            state.TrackedContext.ShouldBeNull();
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class StubContext
    {
        public string? StringValue { get; set; }
    }
}