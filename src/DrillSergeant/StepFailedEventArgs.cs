using System;

namespace DrillSergeant
{
    public class StepFailedEventArgs : EventArgs
    {
        public StepFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
