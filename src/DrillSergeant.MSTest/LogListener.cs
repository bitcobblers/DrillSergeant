﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace DrillSergeant.MSTest;

[SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
internal class LogListener : IDisposable
{
    private static ThreadSafeStringWriter? _redirectTraceDebug;
    private static BehaviorTraceListener? _traceListener;
    private static int _traceCount;
    private static readonly object TraceLock = new();

    private readonly TextWriter _originalStdOut;
    private readonly TextWriter _originalStdErr;

    private readonly ThreadSafeStringWriter _redirectStdOut = new(CultureInfo.InvariantCulture, "stdout");
    private readonly ThreadSafeStringWriter _redirectStdErr = new(CultureInfo.InvariantCulture, "stderr");

    private bool _disposed;

    public LogListener(bool captureTraceLogs)
    {
        Logger.OnLogMessage += _redirectStdOut.WriteLine;

        _originalStdOut = Console.Out;
        _originalStdErr = Console.Error;

        Console.SetOut(_redirectStdOut);
        Console.SetError(_redirectStdErr);

        if (captureTraceLogs == false)
        {
            return;
        }

        lock (TraceLock)
        {
            if (_traceCount == 0)
            {
                _redirectTraceDebug = new ThreadSafeStringWriter(CultureInfo.InvariantCulture, "trace");
                _traceListener = new BehaviorTraceListener(_redirectTraceDebug);
                Trace.Listeners.Add(_traceListener);
            }

            _traceCount++;
        }
    }

    [ExcludeFromCodeCoverage]
    ~LogListener() => Dispose(disposing: false);

    public ThreadSafeStringWriter StdOut => _redirectStdOut;

    // ReSharper disable once UnusedMember.Global
    public ThreadSafeStringWriter StdErr => _redirectStdErr;

    public string GetAndClearStdOut() => _redirectStdOut.ToStringAndClear();

    public string GetAndClearStdErr() => _redirectStdErr.ToStringAndClear();

    // ReSharper disable once MemberCanBeMadeStatic.Global
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
                if (_traceCount == 1)
                {
                    Trace.Listeners.Remove(_traceListener);
                    _traceListener?.Dispose();
                    _redirectTraceDebug?.Dispose();
                }

                _traceCount--;
            }
        }

        _disposed = true;
    }
}
