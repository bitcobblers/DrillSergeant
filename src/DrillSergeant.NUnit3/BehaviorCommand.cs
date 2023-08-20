using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace DrillSergeant.NUnit3;

internal class BehaviorCommand : TestCommand
{
    public BehaviorCommand(Test test)
        : base(test)
    {
    }

    /// <inheritdoc />
    public override TestResult Execute(TestExecutionContext context) =>
        ExecuteAsync(context).GetAwaiter().GetResult();

    /// <summary>
    /// Executes the command in an async context.
    /// </summary>
    /// <param name="context">The test execution context.</param>
    /// <returns>The result from running the test.</returns>
    private static async Task<TestResult> ExecuteAsync(TestExecutionContext context)
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
