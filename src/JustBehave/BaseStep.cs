using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JustBehave;

public abstract class BaseStep<TContext, TInput> : IStep
{
    public virtual string Verb { get; protected set; } = "<unknown>";

    public virtual string Name { get; protected set; } = "<untitled step>";

    ~BaseStep()
    {
        this.Dispose(disposing: false);
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public abstract object? Execute(object context, object input, IDependencyResolver resolver);

    protected virtual object?[] ResolveParameters(IDependencyResolver resolver, object context, object input, ParameterInfo[] parameters)
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

        return (from p in parameters
                select resolve(p)).ToArray();
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    internal static bool IsAsync(MethodInfo method) =>
        method.ReturnType.Name == typeof(Task).Name || method.ReturnType.Name == typeof(Task<>).Name;
}
