using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Core;

public class BehaviorTestCase : XunitTestCase
{
    [Obsolete("Called by the de-serializer", true)]
    public BehaviorTestCase()
    {
    }

    public BehaviorTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod, object[] testMethodArguments) 
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
    {
    }

    public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
    {
        var runner = new BehaviorTestCaseRunner(
            this,
            DisplayName,
            SkipReason,
            constructorArguments,
            TestMethodArguments,
            messageBus,
            aggregator,
            cancellationTokenSource
            );

        return runner.RunAsync();
    }
}
