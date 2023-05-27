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

    public virtual async Task Execute(object context, object input)
    {
        var handler = this.PickHandler();
        var parameters = this.ResolveParameters(context, input, handler.Method.GetParameters());
        dynamic result = handler.DynamicInvoke(parameters)!;

        if (IsAsync(handler.Method))
        {
            await result;
        }
    }

    internal virtual object?[] ResolveParameters(object context, object input, ParameterInfo[] parameters)
    {
        var contextType = context.GetType();
        var inputType = input.GetType();

        object? resolve(ParameterInfo parameter, int index)
        {
            if (index == 0)
            {
                return context;
            }

            //if (parameter.ParameterType == contextType ||
            //    contextType.GetInterfaces().Contains(parameter.ParameterType))
            //{
            //    return context;
            //}
            if (parameter.ParameterType == inputType ||
                inputType.GetInterfaces().Contains(parameter.ParameterType))
            {
                return input;
            }

            return null;
        }

        return parameters.Select(resolve).ToArray();
    }

    protected abstract Delegate PickHandler();

    [ExcludeFromCodeCoverage]
    protected virtual void Dispose(bool disposing)
    {
    }

    internal static bool IsAsync(MethodInfo method) =>
        method.ReturnType.Name == typeof(Task).Name || method.ReturnType.Name == typeof(Task<>).Name;
}
