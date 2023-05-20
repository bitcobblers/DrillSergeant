using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DrillSergeant;

public abstract class BaseStep<TContext, TInput> : IStep
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

    public abstract Task Execute(object context, object input, IDependencyResolver resolver);

    internal virtual object?[] ResolveParameters(IDependencyResolver resolver, object context, object input, ParameterInfo[] parameters)
    {
        object resolve(ParameterInfo parameter)
        {
            if (parameter.ParameterType == context.GetType())
            {
                return context;
            }
            else if (parameter.ParameterType == input.GetType())
            {
                return input;
            }

            return resolver.Resolve(parameter.ParameterType);
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
