using FakeItEasy;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class LambdaStepTests
{
    public class NamedMethod : LambdaStepTests
    {
        public record Context();
        public record Input();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void SettingBlankNameDoesNotChangeExistingValue(string blankValue)
        {
            // Arrange.
            var step = new LambdaStep<Context, Input>("Test").Named("expected");

            // Act.
            step.Named(blankValue);

            // Assert.
            step.Name.ShouldBe("expected");
        }
    }

    public class ExecuteMethod : LambdaStepTests
    {
        public record Context(int Value = 0);
        public record Input();

        [Fact]
        public async Task NonAsyncHandlerWithReturnReturnsValue()
        {
            // Arrange.
            var step = new LambdaStep<Context, Input>("Test").Handle((c, i) => c with { Value = 1 });
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();
            var expected = new Context(1);

            // Act.
            var result = await step.Execute(context, input, resolver);

            // Assert.
            result.ShouldBe(expected);
        }

        [Fact]
        public async Task NonAsyncHandlerWithNoReturnReturnsNull()
        {
            // Arrange.
            var step = new LambdaStep<Context, Input>("Test").Handle((c, i) => { });
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();
            var expected = new Context(1);

            // Act.
            var result = await step.Execute(context, input, resolver);

            // Assert.
            result.ShouldBe(null);
        }

        [Fact]
        public async Task AsyncHandlerWithReturnReturnsValue()
        {
            // Arrange.
            var step = new LambdaStep<Context, Input>("Test").Handle((c, i) => Task.FromResult(c with { Value = 1 }));
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();
            var expected = new Context(1);

            // Act.
            var result = await step.Execute(context, input, resolver);

            // Assert.
            result.ShouldBe(expected);
        }

        [Fact]
        public async Task AsyncHandlerWithNoReturnReturnsNull()
        {
            // Arrange.
            var step = new LambdaStep<Context, Input>("Test").Handle((c, i) => Task.CompletedTask);
            var resolver = A.Fake<IDependencyResolver>();
            var context = new Context();
            var input = new Input();
            var expected = new Context(1);

            // Act.
            var result = await step.Execute(context, input, resolver);

            // Assert.
            result.ShouldBeNull();
        }
    }
}
