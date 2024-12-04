using System.Collections;

namespace DrillSergeant.Tests;

public class LambdaStepBuilderTests
{
    public class StubLambdaStep : LambdaStepBuilder<StubLambdaStep>;

    public class SetNameMethod : LambdaStepBuilderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void SettingBlankNameDoesNotChangeExistingValue(string? blankValue)
        {
            // Arrange.
            var step = new StubLambdaStep().SetName("expected");

            // Act.
            step.SetName(blankValue);

            // Assert.
            step.Name.ShouldBe("expected");
        }
    }

    public class SetVerbMethod : LambdaStepBuilderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void SettingBlankVerbDoesNotChangeExistingValue(string? blankValue)
        {
            // Arrange.
            var step = new StubLambdaStep().SetVerb("expected");

            // Act.
            step.SetVerb(blankValue);

            // Assert.
            step.Verb.ShouldBe("expected");
        }
    }

    public class SkipMethod : LambdaStepBuilderTests
    {
        [Fact]
        public void SkipDisabledByDefault()
        {
            // Arrange.
            var step = new StubLambdaStep();

            // Act.
            var result = step.ShouldSkip;

            // Assert.
            result.ShouldBeFalse();
        }

        [Fact]
        public void NullPredicateEnablesSkip()
        {
            // Arrange.
            var step = new StubLambdaStep();

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
            var step = new StubLambdaStep();

            // Act.
            step.Skip(() => true);
            var result = step.ShouldSkip;

            // Assert.
            result.ShouldBeTrue();
        }
    }
}
