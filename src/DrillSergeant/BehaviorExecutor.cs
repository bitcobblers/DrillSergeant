using DrillSergeant.Reporting;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace DrillSergeant
{
    public class BehaviorExecutor
    {
        public event EventHandler<StepFailedEventArgs> StepFailed = delegate {};

        private readonly ITestReporter _reporter;

        public BehaviorExecutor(ITestReporter reporter) => _reporter = reporter;

        public async Task<IBehavior> LoadBehavior(object instance, MethodInfo method, object?[] parameters)
        {
            BehaviorBuilder.Clear();

            if (IsAsync(method))
            {
                dynamic asyncResult = method.Invoke(instance, parameters)!;
                await asyncResult;
            }
            else
            {
                method.Invoke(instance, parameters);
            }

            return BehaviorBuilder.CurrentBehavior ??
                   throw new InvalidOperationException("Test method did not return a behavior.");
        }

        public async Task Execute(IBehavior behavior)
        {
            bool previousStepFailed = false;

            if (behavior == null)
            {
                throw new InvalidOperationException("Attempted to execute an undefined behavior.");
            }

            _reporter.WriteBlock("Input", behavior.Input);

            foreach (var step in behavior)
            {
                if (previousStepFailed)
                {
                    _reporter.WriteStepResult(step.Verb, step.Name, previousStepFailed, elapsed: 0, success: false, context: null);
                    continue;
                }

                var elapsed = await TimedCall(async () =>
                {
                    try
                    {
                        await step.Execute(behavior.Context, behavior.Input);
                    }
                    catch (Exception ex)
                    {
                        StepFailed(this, new StepFailedEventArgs(ex));
                        previousStepFailed = true;
                    }
                });

                _reporter.WriteStepResult(step.Verb, step.Name, false, elapsed, !previousStepFailed, behavior.LogContext ? behavior.Context : null);
            }
        }

        private static bool IsAsync(MethodInfo method) => 
            method.ReturnType.IsAssignableTo(typeof(Task));

        private static async Task<decimal> TimedCall(Func<Task> task)
        {
            var stopwatch = Stopwatch.StartNew();
            await task();
            return (decimal)stopwatch.Elapsed.TotalSeconds;
        }
    }
}
