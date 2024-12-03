using System.Collections;

namespace DrillSergeant.Tests;

public class LambdaStepTests
{
    public class SetNameMethod : LambdaStepTests
    {
        public class LambdaStep : Facts<DrillSergeant.LambdaStep>;

        public class Facts<TLambdaStep> : SetNameMethod
            where TLambdaStep : LambdaStepBuilder<TLambdaStep>, new()
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData("\t")]
            public void SettingBlankNameDoesNotChangeExistingValue(string? blankValue)
            {
                // Arrange.
                var step = new TLambdaStep().SetName("expected");

                // Act.
                step.SetName(blankValue);

                // Assert.
                step.Name.ShouldBe("expected");
            }
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
}
