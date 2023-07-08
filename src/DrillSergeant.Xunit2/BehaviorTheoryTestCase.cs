using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Xunit2;

internal class BehaviorTheoryTestCase : XunitTheoryTestCase
{
    public BehaviorTheoryTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
    {
    }

    public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
    {
        var runner = new BehaviorTheoryTestCaseRunner(
            this,
            DisplayName,
            SkipReason,
            constructorArguments,
            diagnosticMessageSink,
            messageBus,
            aggregator,
            cancellationTokenSource);

        return runner.RunAsync();
    }
}
