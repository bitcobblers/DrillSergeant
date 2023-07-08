using System.Text;

namespace DrillSergeant.MSTest;

internal class ThreadSafeStringBuilder
{
    private readonly StringBuilder _builder = new();
    private readonly object _sync = new();

    public void Clear() => Wrap(() => _builder.Clear());

    public void AppendLine(string? value) => Wrap(() => _builder.AppendLine(value));

    public override string ToString() => WrapFunc(() => _builder.ToString());

    private void Wrap(Action action)
    {
        lock (_sync)
        {
            action.Invoke();
        }
    }

    private T WrapFunc<T>(Func<T> func)
    {
        lock (_sync)
        {
            return func();
        }
    }
}