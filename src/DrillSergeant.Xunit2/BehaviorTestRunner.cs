using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Xunit2;

internal class BehaviorTestRunner : XunitTestRunner
{
    public BehaviorTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource)
    {
    }

    protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
    {
        (TestOutputHelper sink, DecoyTestOutputHelper decoy) = GetOutputHelper();
        using var reporter = new XunitRawTestReporter(sink, decoy);
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

        for (var i = 0; i < ConstructorArguments.Length; i++)
        {
            if (ConstructorArguments[i] is not TestOutputHelper testOutputHelper)
            {
                continue;
            }

            sink = testOutputHelper;
            ConstructorArguments[i] = decoy;
            break;
        }

        sink ??= new TestOutputHelper();
        sink.Initialize(MessageBus, Test);

        return (sink, decoy);
    }
}
