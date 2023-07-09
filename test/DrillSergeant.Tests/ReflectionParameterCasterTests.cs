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

    public class InstantiateClassMethod : ReflectionParameterCasterTests
    {
        [Fact]
        public void ThrowsInvalidOperationExceptionIfTypeDoesNotContainAnEmptyConstructor()
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Assert.
            Assert.Throws<InvalidOperationException>(
                () => ReflectionParameterCaster.InstantiateClass(source, typeof(TargetWithoutEmptyConstructor)));
        }

        [Fact]
        public void MatchingTypesAreMappedAutomatically()
        {
            // Arrange.
            var caster = new ReflectionParameterCaster();
            var source = new Dictionary<string, object?>
            {
                ["IntValue"] = 1
            };

            // Act.
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateClass(source, typeof(TargetWithParameters));

            // Assert.
            result.IntValue.ShouldBe(1);
        }

        [Fact]
        public void MisMatchedTypesAreIgnoredWhenMapping()
        {
            // Arrange.
            var caster = new ReflectionParameterCaster();
            var source = new Dictionary<string, object?>
            {
                ["StringValue"] = 1
            };

            // Act.
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateClass(source, typeof(TargetWithParameters));

            // Assert.
            result.StringValue.ShouldBeNull();
        }

        [Fact]
        public void MatchingSupportsCovariantTypes()
        {
            // Arrange.
            var caster = new ReflectionParameterCaster();
            var source = new Dictionary<string, object?>
            {
                ["CovariantTypeValue"] = new StubDerivedType()
            };

            // Act.
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateClass(source, typeof(TargetWithParameters));

            // Assert.
            result.CovariantTypeValue.ShouldNotBeNull();
        }

        [Fact]
        public void MatchingDoesNotSupportContravariantTypes()
        {
            // Arrange.
            var caster = new ReflectionParameterCaster();
            var source = new Dictionary<string, object?>
            {
                ["ContravariantTypeValue"] = new StubBaseType()
            };

            // Act.
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateClass(source, typeof(TargetWithParameters));

            // Assert.
            result.CovariantTypeValue.ShouldBeNull();
        }

        [Fact]
        public void NullPropertyIsUnset()
        {
            // Arrange.
            var caster = new ReflectionParameterCaster();
            var source = new Dictionary<string, object?>
            {
                ["StringValue"] = null
            };

            // Act.
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateClass(source, typeof(TargetWithParameters));

            // Assert.
            result.CovariantTypeValue.ShouldBeNull();
        }

        public record TargetWithoutEmptyConstructor(int Ignored);

        public class TargetWithParameters
        {
            public int IntValue { get; init; }
            public string? StringValue { get; init; }
            public StubBaseType? CovariantTypeValue { get; init; }
            public StubDerivedType? ContravariantTypeValue { get; init; }
        }

        public class StubBaseType { }
        public class StubDerivedType : StubBaseType { }
    }

    public class InstantiateRecordMethod : ReflectionParameterCasterTests
    {
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
            var result = (RecordWithParameters)ReflectionParameterCaster.InstantiateRecord(source, typeof(RecordWithParameters));

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
            var result = (RecordWithOptionalParameters)ReflectionParameterCaster.InstantiateRecord(source, typeof(RecordWithOptionalParameters));

            // Assert.
            result.IntValue.ShouldBe(1);
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
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateRecord(source, typeof(TargetWithParameters));

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
            var result = (TargetWithParameters)ReflectionParameterCaster.InstantiateRecord(source, typeof(TargetWithParameters));

            // Assert.
            result.CovariantTypeValue.ShouldBeNull();
        }


        public record RecordWithParameters(int IntValue, string StringValue);
        
        public record RecordWithOptionalParameters(int IntValue, string? StringValue);

        public record TargetWithParameters(
            int IntValue,
            string? StringValue,
            StubBaseType? CovariantTypeValue,
            StubDerivedType? ContravariantTypeValue);

        public record StubBaseType { }
        public record StubDerivedType : StubBaseType { }
    }

    public class IsRecordMethod : ReflectionParameterCasterTests
    {
        [Theory]
        [InlineData(typeof(CompleteRecord))]
        [InlineData(typeof(PartialRecord))]
        public void ValidScenarios(Type type)
        {
            // Act.
            bool result = ReflectionParameterCaster.IsRecord(type);

            // Assert.
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(typeof(EmptyRecord))]
        [InlineData(typeof(NonRecord))]
        public void InvalidScenarios(Type type)
        {            
            // Act.
            bool result = ReflectionParameterCaster.IsRecord(type);

            // Assert.
            result.ShouldBeFalse();
        }

        public record CompleteRecord(int IntValue, string StringValue);

        public record PartialRecord(int IntValue)
        {
            public string? StringValue { get; init; }
        }

        public record EmptyRecord();

        public class NonRecord
        {
            public NonRecord(string stringValue)
            {
            }

            public int IntValue { get; set; }
        }
    }
}
