using Shouldly;
using System.Dynamic;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class LambdaStepTests
{
    public class NamedMethod : LambdaStepTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void SettingBlankNameDoesNotChangeExistingValue(string blankValue)
        {
            // Arrange.
            var step = new LambdaStep("Test").Named("expected");

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
            var step = new LambdaStep("Test").Handle((c, i) =>
            {
                c.Value = 1;
            });

            dynamic context = new ExpandoObject();
            dynamic input = new ExpandoObject();

            // Act.
            await step.Execute(context, input);

            // Assert.
            ((object)context.Value).ShouldBe(1);
        }

        [Fact]
        public async Task AsyncHandlerWithReturnReturnsValue()
        {
            // Arrange.
            var step = new LambdaStep("Test").Handle((c, i) =>
            {
                c.Value = 1;
                return Task.CompletedTask;
            });

            dynamic context = new ExpandoObject();
            dynamic input = new ExpandoObject();

            // Act.
            await step.Execute(context, input);

            // Assert.
            ((object)context.Value).ShouldBe(1);
        }
    }
}
