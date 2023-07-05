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
        private readonly object _instance;
        private readonly MethodInfo _method;
        private readonly object?[] _parameters;

        public BehaviorExecutor(ITestReporter reporter, object instance, MethodInfo method, object?[] parameters)
        {
            _reporter = reporter;
            _instance = instance;
            _method = method;
            _parameters = parameters;
        }

        /// <summary>
        /// Gets the behavior to execute.
        /// </summary>
        public IBehavior? Behavior { get; private set; }

        public async Task LoadBehavior()
        {
            if (IsAsync(_method))
            {
                var genericArguments = _method.ReturnType.GetGenericArguments();

                if (genericArguments.Length == 1 && genericArguments[0].IsAssignableTo(typeof(IBehavior)))
                {
                    dynamic asyncResult = _method.Invoke(_instance, _parameters)!;
                    Behavior = (IBehavior)await asyncResult;
                }
            }
            else
            {
                var returnType = _method.ReturnType;

                if (returnType.IsAssignableTo(typeof(IBehavior)))
                {
                    Behavior = (IBehavior?)_method.Invoke(_instance, _parameters);
                }
            }

            if (Behavior == null)
            {
                throw new InvalidOperationException("Test method did not return a behavior.");
            }
        }

        public async Task Execute()
        {
            bool previousStepFailed = false;

            if (Behavior == null)
            {
                throw new InvalidOperationException("Attempted to execute an undefined behavior.");
            }

            _reporter.WriteBlock("Input", Behavior.Input);

            foreach (var step in Behavior)
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
                        await step.Execute(Behavior.Context, Behavior.Input);
                    }
                    catch (Exception ex)
                    {
                        StepFailed(this, new StepFailedEventArgs(ex));
                        previousStepFailed = true;
                    }
                });

                _reporter.WriteStepResult(step.Verb, step.Name, false, elapsed, !previousStepFailed, Behavior.LogContext ? Behavior.Context : null);
            }
        }

        private static bool IsAsync(MethodInfo method) =>
            method.ReturnType.Name == nameof(Task) || method.ReturnType.Name == typeof(Task<>).Name;

        private static async Task<decimal> TimedCall(Func<Task> task)
        {
            var stopwatch = Stopwatch.StartNew();
            await task();
            return (decimal)stopwatch.Elapsed.TotalSeconds;
        }
    }
}
