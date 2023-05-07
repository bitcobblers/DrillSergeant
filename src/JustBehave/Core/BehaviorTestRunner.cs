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

    protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
    {
        var testOutputHelper = GetOutputHelper();
        var executionTime = await InvokeTestMethodAsync(aggregator, testOutputHelper);
        var output = testOutputHelper.Output;
        
        testOutputHelper.Uninitialize();

        return Tuple.Create(executionTime, output);
    }

    private Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator, ITestOutputHelper outputHelper)
    {
        var invoker = new BehaviorTestInvoker(
            outputHelper,
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

    private TestOutputHelper GetOutputHelper()
    {
        TestOutputHelper? result = null;

        foreach (object obj in ConstructorArguments)
        {
            if (obj is TestOutputHelper testOutputHelper)
            {
                result = testOutputHelper;
                break;
            }
        }

        result ??= new TestOutputHelper();
        result.Initialize(MessageBus, Test);

        return result;
    }
}
