using Shouldly;
using System.Dynamic;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class LambdaStepTests
{
    public class SetNameMethod : LambdaStepTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void SettingBlankNameDoesNotChangeExistingValue(string blankValue)
        {
            // Arrange.
            var step = new LambdaStep("Test").SetName("expected");

            // Act.
            step.SetName(blankValue);

            // Assert.
            step.Name.ShouldBe("expected");
        }
    }

    public class SetVerbMethod : LambdaStepTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void SettingBlankVerbDoesNotChangeExistingValue(string? blankValue)
        {
            // Arrange.
            var step = new LambdaStep().SetVerb("expected");

            // Act.
            step.SetVerb(blankValue);

            // Assert.
            step.Verb.ShouldBe("expected");
        }
    }

    public class SkipMethod : LambdaStepTests
    {
        [Fact]
        public void SkipDisabledByDefault()
        {
            // Arrange.
            var step = new LambdaStep();

            // Act.
            var result = step.ShouldSkip;

            // Assert.
            result.ShouldBeFalse();
        }

        [Fact]
        public void NullPredicateEnablesSkip()
        {
            // Arrange.
            var step = new LambdaStep();

            // Act.
            step.Skip();
            var result = step.ShouldSkip;

            // Assert.
            result.ShouldBeTrue();
        }

        [Fact]
        public void NotNullPredicateEnablesSkip()
        {
            // Arrange.
            var step = new LambdaStep();

            // Act.
            step.Skip(() => true);
            var result = step.ShouldSkip;

            // Assert.
            result.ShouldBeTrue();
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
            var step = new LambdaStep("Test").HandleAsync((c, i) =>
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
