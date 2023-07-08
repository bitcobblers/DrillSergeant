using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace DrillSergeant.MSTest;

public class LogListener : IDisposable
{
    private static ThreadSafeStringWriter? _redirectTraceDebug;
    private static BehaviorTraceListener? _traceListener;
    private static int _traceCount = 0;
    private static readonly object TraceLock = new ();

    private readonly TextWriter _originalStdOut;
    private readonly TextWriter _originalStdErr;
    
    private readonly ThreadSafeStringWriter _redirectStdOut = new(CultureInfo.InvariantCulture, "stdout");
    private readonly ThreadSafeStringWriter _redirectStdErr = new(CultureInfo.InvariantCulture, "stderr");

    private bool _disposed;

    public LogListener()
    {
        Logger.OnLogMessage += _redirectStdOut.WriteLine;

        _originalStdOut = Console.Out;
        _originalStdErr = Console.Error;

        Console.SetOut(_redirectStdOut);
        Console.SetError(_redirectStdErr);

        lock (TraceLock)
        {
            if (_traceCount == 0)
            {
                _redirectTraceDebug = new(CultureInfo.InvariantCulture, "trace");
                _traceListener = new BehaviorTraceListener(_redirectTraceDebug);
                Trace.Listeners.Add(_traceListener);
            }

            _traceCount++;
        }
    }

    ~LogListener() => Dispose(disposing: false);

    public string GetAndClearStdOut() => _redirectStdOut.ToStringAndClear();
    public string GetAndClearStdErr() => _redirectStdErr.ToStringAndClear();
    public string GetAndClearTrace() => _redirectTraceDebug?.ToStringAndClear() ?? string.Empty;

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            Console.SetOut(_originalStdOut);
            Console.SetError(_originalStdErr);

            _redirectStdOut.Dispose();
            _redirectStdErr.Dispose();

            lock (TraceLock)
            {
                if (_traceCount == 0)
                {
                    Trace.Listeners.Remove(_traceListener);
                    _traceListener?.Dispose();
                    _redirectTraceDebug?.Dispose();
                }
                else
                {
                    _traceCount--;
                }
            }
        }

        _disposed = true;
    }
}
