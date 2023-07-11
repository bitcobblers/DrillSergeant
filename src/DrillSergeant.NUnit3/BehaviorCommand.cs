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

    public override TestResult Execute(TestExecutionContext context) =>
        ExecuteAsync(context).GetAwaiter().GetResult();

    private async Task<TestResult> ExecuteAsync(TestExecutionContext context)
    {
        var reporter = new RawTestReporter(context.OutWriter);
        var executor = new BehaviorExecutor(reporter);

        var obj = context.TestObject;
        var method = context.CurrentTest.Method!.MethodInfo;
        var args = context.CurrentTest.Arguments;

        executor.StepFailed += (_, e) =>
            context.CurrentResult.RecordException(e.Exception, FailureSite.Test);

        context.CurrentResult.SetResult(ResultState.Success);

        using var behavior = await executor.LoadBehavior(obj, method, args);

        if (behavior == null)
        {
            context.CurrentResult.SetResult(ResultState.Error, $"Unable to load the behavior {method.Name}.");
            return context.CurrentResult;
        }

        try
        {
            await executor.Execute(behavior, CancellationToken.None, context.TestCaseTimeout);
        }
        catch (BehaviorTimeoutException ex)
        {
            context.CurrentResult.SetResult(ResultState.Error, ex.Message);
            context.CurrentResult.RecordException(ex);
        }

        if (context.CurrentResult.AssertionResults.Count > 0)
        {
            context.CurrentResult.RecordTestCompletion();
        }

        return context.CurrentResult;
    }
}
