using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace DrillSergeant.Tests;

public class ReflectionParameterCasterTests
{
    public class CastMethod : ReflectionParameterCasterTests
    {
        [Fact]
        public void NoConversionIsPerformedWhenRawObjectIsPassed()
        {
            // Arrange.
            var caster = new ReflectionParameterCaster();
            var source = new Dictionary<string, object?>();

            // Act.
            var result = caster.Cast(source, typeof(object));

            // Assert.
            result.ShouldBeSameAs(source);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(int[]))]
        public void CastingToPrimitiveThrowsInvalidOperationException(Type type)
        {
            // Arrange.
            var caster = new ReflectionParameterCaster();
            var source = new Dictionary<string, object?>();

            // Assert.
            Assert.Throws<InvalidOperationException>(() => caster.Cast(source, type));
        }
    }

    public class InstantiateInstanceMethod : ReflectionParameterCasterTests
    {
        [Fact]
        public void NoPublicConstructorsThrowsInvalidOperationException()
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Assert.
            Assert.Throws<InvalidOperationException>(() =>
                ReflectionParameterCaster.InstantiateInstance(source, typeof(ClassWithoutPublicConstructors)));
        }

        [Fact]
        public void MultipleConstructorsThrowsInvalidOperationException()
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Assert.
            Assert.Throws<InvalidOperationException>(() =>
                ReflectionParameterCaster.InstantiateInstance(source, typeof(ClassWithMultipleConstructors)));
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
            var result = (RecordWithParameters)ReflectionParameterCaster.InstantiateInstance(source, typeof(RecordWithParameters));

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
            var result = (RecordWithOptionalParameters)ReflectionParameterCaster.InstantiateInstance(source, typeof(RecordWithOptionalParameters));

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
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

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
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

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
            var result = (RecordWithOptionalParameters)ReflectionParameterCaster.InstantiateInstance(source, typeof(RecordWithOptionalParameters));

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
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

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
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

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
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateInstance(source, typeof(TargetWithParameters));

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

            public ClassWithMultipleConstructors(int ignored)
            {
            }
        }


        public class StubBaseType { }
        public class StubDerivedType : StubBaseType { }
    }
}
