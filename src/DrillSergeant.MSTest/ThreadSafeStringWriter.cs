namespace DrillSergeant.MSTest;

public class ThreadSafeStringWriter : StringWriter
{
    private static readonly AsyncLocal<Dictionary<string, ThreadSafeStringBuilder>> State = new();

    private static readonly object GlobalSync = new();
    private readonly string _kind;

    public ThreadSafeStringWriter(IFormatProvider formatProvider, string kind)
        : base(formatProvider)
    {
        _kind = kind;

        // Force the writer to be pulled into this level of the async stack.
        _ = ToStringAndClear();
    }

    public override void WriteLine(string? value) => GetOrAdd().AppendLine(value);

    public override string ToString()
    {
        try
        {
            return GetOrNull()?.ToString() ?? string.Empty;
        }
        catch (ObjectDisposedException)
        {
            return string.Empty;
        }
    }

    public string ToStringAndClear()
    {
        try
        {
            var builder = GetOrAdd();
            var value = builder?.ToString();

            builder?.Clear();
            return value ?? string.Empty;
        }
        catch (ObjectDisposedException)
        {
            return string.Empty;
        }
    }

    private ThreadSafeStringBuilder GetOrAdd()
    {
        lock (GlobalSync)
        {
            State.Value ??= new();

            if (State.Value!.TryGetValue(_kind, out var builder))
            {
                return builder;
            }

            var newBuilder = new ThreadSafeStringBuilder();
            State.Value.Add(_kind, newBuilder);

            return newBuilder;
        }
    }

    private ThreadSafeStringBuilder? GetOrNull()
    {
        lock (GlobalSync)
        {
            if (State.Value == null)
            {
                return null;
            }

            return State.Value.TryGetValue(_kind, out var builder) ? builder : null;
        }
    }
}
