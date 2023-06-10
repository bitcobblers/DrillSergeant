﻿using Newtonsoft.Json;
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

    public virtual async Task Execute(IDictionary<string, object?> context, IDictionary<string, object?> input)
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
            UpdateContext(context, resolvedContext);
        }
    }

    protected abstract Delegate PickHandler();

    [ExcludeFromCodeCoverage]
    protected virtual void Dispose(bool disposing)
    {
    }

    internal static object?[] ResolveParameters(IDictionary<string, object?> context, IDictionary<string, object?> input, ParameterInfo[] parameters)
    {
        var result = new object[parameters.Length];

        if(result.Length>0)
        {
            result[0] = DynamicCast(context, parameters[0].ParameterType);
        }

        if(result.Length>1)
        {
            result[1] = DynamicCast(input, parameters[1].ParameterType);
        }

        return result;
    }

    internal static object DynamicCast(IDictionary<string, object?> source, Type type)
    {
        if (type == typeof(object))
        {
            return source;
        }

        if (type.IsPrimitive ||
            type.IsArray ||
            type == typeof(string))
        {
            throw new InvalidOperationException("Cannot cast to a primitive type.");
        }

        var serialized = JsonConvert.SerializeObject(source);
        var converted = JsonConvert.DeserializeObject(serialized, type)!;

        return converted;
    }

    internal static void UpdateContext(IDictionary<string, object?> context, object changedContext)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        foreach (var property in changedContext.GetType().GetProperties(flags))
        {
            context[property.Name] = property.GetValue(changedContext);
        }
    }

    internal static bool IsAsync(MethodInfo method) =>
        method.ReturnType.Name == typeof(Task).Name || method.ReturnType.Name == typeof(Task<>).Name;
}
