using System;
using System.Threading.Tasks;

namespace DrillSergeant.Tests;

public class BehaviorBuilderTests
{
    public class CurrentProperty : BehaviorBuilderTests
    {
        [Fact]
        public void ThrowsNoCurrentBehaviorExceptionWhenOutsideBuildScope()
        {
            // Assert.
            Assert.Throws<NoCurrentBehaviorException>(() => BehaviorBuilder.Current);
        }
    }

    public class BuildMethod : BehaviorBuilderTests
    {
        [Fact]
        public Task StackSizeIsResetAfterSuccessfulBuild()
        {
            // Arrange.
            var stack = BehaviorBuilder.GetCurrentStack();
            var initialSize = stack.Count;

            // Act.
            BehaviorBuilder.Build(_ => { });

            // Assert.
            stack.Count.ShouldBe(initialSize);

            return Task.CompletedTask;
        }

        [Fact]
        public Task StackSizeIsResetWhenExceptionIsThrownDuringBuild()
        {
            // Arrange.
            var stack = BehaviorBuilder.GetCurrentStack();
            var initialSize = stack.Count;

            // Act.
            try
            {
                BehaviorBuilder.Build(_ => throw new Exception("ignored"));
            }
            catch
            {
                // Deliberately swallow.
            }

            // Assert.
            stack.Count.ShouldBe(initialSize);

            return Task.CompletedTask;
        }
    }

    public class BuildAsyncMethod : BehaviorBuilderTests
    {
        [Fact]
        public async Task StackSizeIsResetAfterSuccessfulBuild()
        {
            // Arrange.
            var stack = BehaviorBuilder.GetCurrentStack();
            var initialSize = stack.Count;

            // Act.
            await BehaviorBuilder.BuildAsync(Task.FromResult);

            // Assert.
            stack.Count.ShouldBe(initialSize);
        }

        [Fact]
        public async Task StackSizeIsResetWhenExceptionIsThrownDuringBuild()
        {
            // Arrange.
            var stack = BehaviorBuilder.GetCurrentStack();
            var initialSize = stack.Count;

            // Act.
            try
            {
                await BehaviorBuilder.BuildAsync(_ => throw new Exception("ignored"));
            }
            catch
            {
                // Deliberately swallow.
            }

            // Assert.
            stack.Count.ShouldBe(initialSize);
        }
    }
}