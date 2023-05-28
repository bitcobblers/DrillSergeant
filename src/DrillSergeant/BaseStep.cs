using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        var handler = PickHandler();
        var parameters = ResolveParameters(context, input, handler.Method.GetParameters());
        var resolvedContext = parameters.FirstOrDefault() ?? context;
        dynamic result = handler.DynamicInvoke(parameters)!;

        if (IsAsync(handler.Method))
        {
            await result;
        }

        if (object.ReferenceEquals(context, resolvedContext) == false)
        {
            var unpacked = UnpackContext(resolvedContext);
            UpdateContext(context, unpacked);
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
                return CastContext((IDictionary<string, object>)context, parameter.ParameterType);
            }

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

    internal static object CastContext(IDictionary<string, object> source, Type contextType)
    {
        if (contextType == typeof(object))
        {
            return source;
        }

        if (contextType.IsInterface == false)
        {
            var serialized = JsonConvert.SerializeObject(source);
            var converted = JsonConvert.DeserializeObject(serialized, contextType)!;

            return converted;
        }

        return source;
    }

    internal static IDictionary<string, object> UnpackContext(object context)
    {
        var result = new Dictionary<string, object>();
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        foreach (var property in context.GetType().GetProperties(flags))
        {
            result.Add(property.Name, property.GetValue(context)!);
        }

        return result;
    }

    internal static void UpdateContext(object context, object changedContext)
    {
        foreach (var (k, v) in (IDictionary<string, object>)changedContext)
        {
            ((IDictionary<string, object>)context)[k] = v;
        }
    }

    internal static bool IsAsync(MethodInfo method) =>
        method.ReturnType.Name == typeof(Task).Name || method.ReturnType.Name == typeof(Task<>).Name;
}
