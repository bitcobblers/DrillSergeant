﻿using DrillSergeant.Reporting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Core;

internal class BehaviorTestRunner : XunitTestRunner
{
    public BehaviorTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource)
    {
    }

    protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
    {
        var (sink, decoy) = GetOutputHelper();
        using var reporter = new RawTestReporter(sink, decoy, Test);
        var executionTime = await InvokeTestMethodAsync(aggregator, reporter);
        var output = reporter.Output;

        return Tuple.Create(executionTime, output);
    }

    private Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator, ITestReporter reporter)
    {
        var invoker = new BehaviorTestInvoker(
            reporter,
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

    private (TestOutputHelper, DecoyTestOutputHelper) GetOutputHelper()
    {
        TestOutputHelper? sink = null;
        DecoyTestOutputHelper decoy = new();

        for (int i = 0; i < ConstructorArguments.Length; i++)
        {
            if (ConstructorArguments[i] is TestOutputHelper testOutputHelper)
            {
                sink = testOutputHelper;
                ConstructorArguments[i] = decoy;
                break;
            }
        }

        sink ??= new TestOutputHelper();
        sink.Initialize(MessageBus, Test);

        return (sink, decoy);
    }
}
