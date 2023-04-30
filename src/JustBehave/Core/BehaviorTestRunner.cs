using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace JustBehave.Core;

public class BehaviorTestRunner : XunitTestRunner
{
    public BehaviorTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) 
        : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource)
    {
    }

    protected override Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
    {
        var invoker = new BehaviorTestInvoker(
            Test, 
            MessageBus, 
            TestClass, 
            ConstructorArguments, 
            TestMethod, 
            TestMethodArguments, 
            BeforeAfterAttributes, 
            aggregator, 
            CancellationTokenSource);

        return invoker.RunAsync();
    }
}
