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
        public record Context
        {
            public int Value { get; set; }
        }

        public record Input();

        [Fact]
        public async Task NonAsyncHandlerWithReturnReturnsValue()
        {
            // Arrange.
            var step = new LambdaStep<Context, Input>("Test").Handle((c, i) => c.Value = 1);
            var context = new Context();
            var input = new Input();

            // Act.
            await step.Execute(context, input);

            // Assert.
            context.Value.ShouldBe(1);
        }

        [Fact]
        public async Task AsyncHandlerWithReturnReturnsValue()
        {
            // Arrange.
            var step = new LambdaStep<Context, Input>("Test").Handle((c, i) =>
            {
                c.Value = 1;
                return Task.CompletedTask;
            });

            var context = new Context();
            var input = new Input();

            // Act.
            await step.Execute(context, input);

            // Assert.
            context.Value.ShouldBe(1);
        }
    }
}
