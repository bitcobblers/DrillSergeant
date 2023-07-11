﻿using Shouldly;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable IDE0060
#pragma warning disable IDE0051

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
        public record Context()
        {
            public int IntValue { get; init; }
            public string? StringValue { get; init; }
        }

        public record Input
        {
            public int Value { get; init; }
        }

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
            var expected = new Context
            {
                IntValue = 1,
                StringValue = "expected"
            };

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
            var expected = new Input { Value = 1 };
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

    private static MethodInfo GetMethod(IReflect type, string name)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        return type.GetMethod(name, flags)!;
    }
}
