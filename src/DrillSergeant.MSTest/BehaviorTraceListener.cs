using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant.MSTest;

[ExcludeFromCodeCoverage]
public class BehaviorTraceListener : TraceListener
{
    private readonly TextWriter _target;

    public BehaviorTraceListener(TextWriter target) => _target = target;

    public override void Write(string? message) => _target.Write(message);

    public override void WriteLine(string? message) => _target.WriteLine(message);
}