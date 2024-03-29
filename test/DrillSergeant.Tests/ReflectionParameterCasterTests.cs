﻿namespace DrillSergeant.Tests;

public class ParameterCasterTests
{
    public class CastMethod : ParameterCasterTests
    {
        [Fact]
        public void NoConversionIsPerformedWhenRawObjectIsPassed()
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Act.
            var result = ParameterCaster.Cast(source, typeof(object));

            // Assert.
            result.ShouldBeSameAs(source);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(int[]))]
        public void CastingToPrimitiveThrowsParameterCastFailedException(Type type)
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Assert.
            Assert.Throws<ParameterCastFailedException>(() => ParameterCaster.Cast(source, type));
        }
    }

    public class InstantiateInstanceMethod : ParameterCasterTests
    {
        [Fact]
        public void NoPublicConstructorsParameterCastFailedException()
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Assert.
            Assert.Throws<ParameterCastFailedException>(() =>
                ParameterCaster.InstantiateInstance(source, typeof(ClassWithoutPublicConstructors)));
        }

        [Fact]
        public void MultipleConstructorsThrowsParameterCastFailedException()
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Assert.
            Assert.Throws<ParameterCastFailedException>(() =>
                ParameterCaster.InstantiateInstance(source, typeof(ClassWithMultipleConstructors)));
        }

        [Fact]
        public void InstantiatesRecordWithParameters()
        {
            // Arrange.
            var source = new Dictionary<string, object?>
            {
                ["IntValue"] = 1,
                ["StringValue"] = "expected"
            };

            // Act.
            var result = (RecordWithParameters)ParameterCaster.InstantiateInstance(source, typeof(RecordWithParameters));

            // Assert.
            result.IntValue.ShouldBe(1);
            result.StringValue.ShouldBe("expected");
        }

        [Fact]
        public void SkipsMissingParametersInSource()
        {
            // Arrange.
            var source = new Dictionary<string, object?>
            {
                ["IntValue"] = 1,
            };

            // Act.
            var result = (RecordWithOptionalParameters)ParameterCaster.InstantiateInstance(source, typeof(RecordWithOptionalParameters));

            // Assert.
            result.IntValue.ShouldBe(1);
            result.StringValue.ShouldBeNull();
        }

        [Fact]
        public void MatchingTypesAreMappedAutomatically()
        {
            // Arrange.
            var source = new Dictionary<string, object?>
            {
                ["IntValue"] = 1
            };

            // Act.
            var result = (TargetWithParameters)ParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

            // Assert.
            result.IntValue.ShouldBe(1);
        }

        [Fact]
        public void MisMatchedTypesAreIgnoredWhenMapping()
        {
            // Arrange.
            var source = new Dictionary<string, object?>
            {
                ["StringValue"] = 1
            };

            // Act.
            var result = (TargetWithParameters)ParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

            // Assert.
            result.StringValue.ShouldBeNull();
        }

        [Fact]
        public void MisMatchedTypesAreIgnoredOnConstructor()
        {
            // Arrange.
            var source = new Dictionary<string, object?>
            {
                ["IntValue"] = 1,
                ["StringValue"] = 1
            };

            // Act.
            var result = (RecordWithOptionalParameters)ParameterCaster.InstantiateInstance(source, typeof(RecordWithOptionalParameters));

            // Assert.
            result.StringValue.ShouldBeNull();
        }

        [Fact]
        public void MatchingSupportsCovariantTypes()
        {
            // Arrange.
            var source = new Dictionary<string, object?>
            {
                ["CovariantTypeValue"] = new StubDerivedType()
            };

            // Act.
            var result = (TargetWithParameters)ParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

            // Assert.
            result.CovariantTypeValue.ShouldNotBeNull();
        }

        [Fact]
        public void MatchingDoesNotSupportContravariantTypes()
        {
            // Arrange.
            var source = new Dictionary<string, object?>
            {
                ["ContravariantTypeValue"] = new StubBaseType()
            };

            // Act.
            var result = (TargetWithParameters)ParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

            // Assert.
            result.CovariantTypeValue.ShouldBeNull();
        }

        [Fact]
        public void NullPropertyIsUnset()
        {
            // Arrange.
            var source = new Dictionary<string, object?>
            {
                ["StringValue"] = null
            };

            // Act.
            var result = (TargetWithParameters)ParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

            // Assert.
            result.CovariantTypeValue.ShouldBeNull();
        }

        public record RecordWithParameters(int IntValue, string StringValue);

        public record RecordWithOptionalParameters(int IntValue, string? StringValue);

        public class TargetWithParameters
        {
            public int IntValue { get; init; }
            public string? StringValue { get; init; }
            public StubBaseType? CovariantTypeValue { get; init; }
            public StubDerivedType? ContravariantTypeValue { get; init; }
        }

        public class ClassWithoutPublicConstructors
        {
            protected ClassWithoutPublicConstructors()
            {

            }
        }

        public class ClassWithMultipleConstructors
        {
            public ClassWithMultipleConstructors()
            {
            }

            // ReSharper disable once UnusedParameter.Local
            public ClassWithMultipleConstructors(int ignored)
            {
            }
        }


        public class StubBaseType { }
        public class StubDerivedType : StubBaseType { }
    }
}
