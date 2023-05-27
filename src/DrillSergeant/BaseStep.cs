using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DrillSergeant;

public abstract class BaseStep : IStep
{
    public virtual string Verb { get; protected set; } = "<unknown>";

    public virtual string Name { get; protected set; } = "<untitled step>";

    [ExcludeFromCodeCoverage]
    ~BaseStep()
    {
        this.Dispose(disposing: false);
    }

    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public abstract Task Execute(object context, object input);

    internal virtual object?[] ResolveParameters(object context, object input, ParameterInfo[] parameters)
    {
        var contextType = context.GetType();
        var inputType = input.GetType();

        object? resolve(ParameterInfo parameter)
        {
            if (parameter.ParameterType == contextType ||
                contextType.GetInterfaces().Contains(parameter.ParameterType))
            {
                return context;
            }
            else if (parameter.ParameterType == inputType ||
                inputType.GetInterfaces().Contains(parameter.ParameterType))
            {
                return input;
            }

            return null;
        }

        return parameters.Select(resolve).ToArray();
    }

    [ExcludeFromCodeCoverage]
    protected virtual void Dispose(bool disposing)
    {
    }

    internal static bool IsAsync(MethodInfo method) =>
        method.ReturnType.Name == typeof(Task).Name || method.ReturnType.Name == typeof(Task<>).Name;
}
