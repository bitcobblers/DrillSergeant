using JustBehave.Core;
using System;
using System.Reflection;
using Xunit;

namespace JustBehave.Tests.Core;

public class BehaviorTestInvokerTests
{
    public class ParseParametersMethod : BehaviorTestInvokerTests
    {
        [Fact]
        public void EmptyParametersReturnsEmptyArray()
        {
            // Arrange.
            var method = GetMethod(nameof(StubWithNoParameters));
            var args = Array.Empty<object>();

            // Act.
            var result = BehaviorTestInvoker.ParseParameters(method, args);

            // Assert.
            Assert.Empty(result);
        }

        [Fact]
        public void EmptyParametersWithSingleInjectReturnsInstantiatedDependency()
        {
            // Arrange.
            var method = GetMethod(nameof(StubWithSingleInjectable));
            var args = Array.Empty<object>();

            // Act.
            var result = BehaviorTestInvoker.ParseParameters(method, args);

            // Assert.
            Assert.NotEmpty(result);
            Assert.NotNull(result[0]);
        }

        [Fact]
        public void TooFewInputsThrowsInvalidOperationException()
        {
            // Arrange.
            var method = GetMethod(nameof(StubWithOneInputAndOneInjectable));
            var args = Array.Empty<object>();

            // Assert.
            Assert.Throws<InvalidOperationException>(() => BehaviorTestInvoker.ParseParameters(method, args));
        }

        [Fact]
        public void TooManyInputsThrowsInvalidOperationException()
        {
            // Arrange.
            var method = GetMethod(nameof(StubWithOneInputAndOneInjectable));
            var args = new object[] { 1, 2 };

            // Assert.
            Assert.Throws<InvalidOperationException>(() => BehaviorTestInvoker.ParseParameters(method, args));
        }

        private MethodInfo GetMethod(string name) => 
            typeof(ParseParametersMethod).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)!;

        private void StubWithNoParameters() { }
        private void StubWithSingleInjectable([Inject] DateTime _) { }
        private void StubWithOneInputAndOneInjectable(int _, [Inject] DateTime ignored) { }
    }
}
