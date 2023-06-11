using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace DrillSergeant.Tests;

public class ReflectionParameterCasterTests
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

    [Fact]
    public void ThrowsInvalidOperationExceptionIfTypeDoesNotContainParameterlessConstructor()
    {
        // Arrange.
        var caster = new ReflectionParameterCaster();
        var source = new Dictionary<string, object?>();

        // Assert.
        Assert.Throws<InvalidOperationException>(() => caster.Cast(source, typeof(TargetWithoutEmptyConstructor)));
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
        var result = (TargetWithParameters)caster.Cast(source, typeof(TargetWithParameters));

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
        var result = (TargetWithParameters)caster.Cast(source, typeof(TargetWithParameters));

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
        var result = (TargetWithParameters)caster.Cast(source, typeof(TargetWithParameters));

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
        var result = (TargetWithParameters)caster.Cast(source, typeof(TargetWithParameters));

        // Assert.
        result.CovariantTypeValue.ShouldBeNull();
    }

    public record TargetWithoutEmptyConstructor(int Ignored);

    public record TargetWithParameters
    {
        public int IntValue { get; init; }
        public string? StringValue { get; init; }
        public StubBaseType? CovariantTypeValue { get; init; }
        public StubDerivedType? ContravariantTypeValue { get; init; }
    }

    public class StubBaseType { }
    public class StubDerivedType : StubBaseType { }
}
