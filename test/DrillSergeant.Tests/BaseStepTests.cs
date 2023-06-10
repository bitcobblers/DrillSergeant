﻿using Shouldly;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DrillSergeant.Tests;

public class BaseStepTests
{
    public class IsAsyncMethod : BaseStepTests
    {
        [Theory]
        [InlineData(nameof(NonAsyncMethod), false)]
        [InlineData(nameof(AsyncWithNoReturn), true)]
        [InlineData(nameof(AsyncWithReturn), true)]
        public void Scenarios(string methodName, bool expected)
        {
            // Arrange.
            var method = GetMethod(typeof(IsAsyncMethod), methodName);

            // Act.
            bool result = BaseStep.IsAsync(method);

            // Assert.
            result.ShouldBe(expected);
        }

        private void NonAsyncMethod() { }
        private Task AsyncWithNoReturn() => Task.CompletedTask;
        private Task<int> AsyncWithReturn() => Task.FromResult(0);
    }

    public class ResolveParametersMethod : BaseStepTests
    {
        public record Context(int IntValue = 0, string? StringValue = null);
        public record Input(int Value);

        [Fact]
        public void ExtraParametersAreNullified()
        {
            // Arrange.
            var context = new Dictionary<string, object?>();
            var input = new Dictionary<string, object?>();
            var method = GetMethod(typeof(ResolveParametersMethod), nameof(StubExecuteMethodWithExtraParameter));

            // Act.
            var parameters = BaseStep.ResolveParameters(context, input, method.GetParameters());

            // Assert.
            parameters[2].ShouldBeNull();
        }

        [Fact]
        public void AutomaticallyCastsDictionaryToContext()
        {
            // Arrange.
            var method = GetMethod(typeof(ResolveParametersMethod), nameof(StubExecuteMethodWithParameters));
            var expected = new Context(1, "expected");
            var context = new Dictionary<string, object?>
            {
                ["IntValue"] = expected.IntValue,
                ["StringValue"] = expected.StringValue
            };

            var input = new Dictionary<string, object?>();

            // Act.
            var result = BaseStep.ResolveParameters(context, input, method.GetParameters());
            var actual = (Context?)result[0];

            // Assert.
            actual.ShouldNotBeNull();
            actual.ShouldBe(expected);
        }

        [Fact]
        public void AutomaticallyCastsDictionaryToInput()
        {
            // Arrange.
            var method = GetMethod(typeof(ResolveParametersMethod), nameof(StubExecuteMethodWithParameters));
            var expected = new Input(1);
            var context = new Dictionary<string, object?>();
            var input = new Dictionary<string, object?>
            {
                ["Value"] = 1
            };

            // Act.
            var result = BaseStep.ResolveParameters(context, input, method.GetParameters());
            var actual = (Input?)result[1];

            // Assert.
            actual.ShouldNotBeNull();
            actual.ShouldBe(expected);
        }

        private void StubExecuteMethodWithParameters(Context context, Input input)
        {
        }

        private void StubExecuteMethodWithExtraParameter(Context context, Input input, object extra)
        {
        }
    }

    public class DynamicCastMethod : BaseStepTests
    {
        [Fact]
        public void CallingWithObjectTypeReturnsSource()
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Act.
            var result = BaseStep.DynamicCast(source, typeof(object));

            // Assert.
            source.ShouldBeSameAs(result);
        }

        [Fact]
        public void PopulatesPropertiesOfObjectType()
        {
            // Arrange.
            var expected = new StubWithProperties
            {
                IntValue = 1,
                StringValue = "expected"
            };

            var source = new Dictionary<string, object?>
            {
                ["IntValue"] = expected.IntValue,
                ["StringValue"] = expected.StringValue
            };

            // Act.
            var result = BaseStep.DynamicCast(source, typeof(StubWithProperties));

            // Assert.
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(string))]
        [InlineData(typeof(int[]))]
        public void CastingToPrimitiveThrowsInvalidOperationException(Type type)
        {
            // Arrange.
            var source = new Dictionary<string, object?>();

            // Act and Assert.
            Assert.Throws<InvalidOperationException>(() => BaseStep.DynamicCast(source, type));
        }

        public record StubWithProperties
        {
            public int IntValue { get; set; }
            public string? StringValue { get; set; }
        }
    }

    public class UpdateContextMethod : BaseStepTests
    {
        [Fact]
        public void UpdatesContextWithNewFields()
        {
            // Arrange.
            var expected = new Dictionary<string, object?>
            {
                ["IntValue"] = 1
            };
            var context = new Dictionary<string, object?>();
            var changedContext = new StubWithValue { IntValue = 1 };

            // Act.
            BaseStep.UpdateContext(context, changedContext);

            // Assert.
            context.ShouldBe(expected);
        }

        [Fact]
        public void UpdatesExistingFieldInContext()
        {
            // Arrange.
            var context = new Dictionary<string, object?>
            {
                ["IntValue"] = -1
            };

            var changedContext = new StubWithValue { IntValue = 1 };

            // Act.
            BaseStep.UpdateContext(context, changedContext);

            // Assert.
            context.ShouldContainKey("IntValue");
            context["IntValue"].ShouldBe(1);
        }

        public class StubWithValue
        {
            public int IntValue { get; set; }

            private int PrivateIntValue { get; set; }

            public static int StaticIntValue { get; set; }
        }
    }

    private static MethodInfo GetMethod(Type type, string name)
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        return type.GetMethod(name, flags)!;
    }
}
