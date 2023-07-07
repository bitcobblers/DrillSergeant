using DrillSergeant.NUnit3.Reporting;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace DrillSergeant.NUnit3;

public class BehaviorCommand : TestCommand
{
    public BehaviorCommand(Test test)
        : base(test)
    {
    }

    public override TestResult Execute(TestExecutionContext context)
    {
        var reporter = new RawTestReporter(context.OutWriter);
        var executor = new BehaviorExecutor(reporter);

        var obj = context.TestObject;
        var method = context.CurrentTest.Method!.MethodInfo;
        var args = context.CurrentTest.Arguments;

        executor.StepFailed += (_, e) => { context.CurrentResult.RecordException(e.Exception); };

        using var behavior = executor.LoadBehavior(obj, method, args).GetAwaiter().GetResult();
        executor.Execute(behavior).Wait();

        context.CurrentResult.SetResult(
            context.CurrentResult.FailCount == 0 || context.CurrentResult.PendingFailures > 0
                ? ResultState.Success
                : ResultState.Failure);

        if (context.CurrentResult.AssertionResults.Count > 0)
        {
            context.CurrentResult.RecordTestCompletion();
        }

        return context.CurrentResult;
    }
}
