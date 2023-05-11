using System;
using System.Linq;
using System.Reflection;

namespace DrillSergeant;

public class VerbStep<TContext, TInput> : BaseStep<TContext, TInput>
{
    public record VerbMethod(MethodInfo Method, object Target, bool IsAsync);

    public VerbStep(string verb)
        : this(verb, string.Empty)
    {
    }

    public VerbStep(string verb, string? name)
    {
        this.Verb = verb;
        this.Name = string.IsNullOrWhiteSpace(name) ? this.GetType().Name : name.Trim();
    }

    public override object? Execute(object context, object input, IDependencyResolver resolver)
    {
        var handler = this.PickHandler();
        var parameters = this.ResolveParameters(resolver, context, input, handler.Method.GetParameters());

        if (handler.IsAsync)
        {
            var taskType = handler.Method.ReturnType;
            var task = handler.Method.Invoke(handler.Target, parameters)!;

            taskType.GetMethod("Wait", Array.Empty<Type>())?.Invoke(task, null);

            if (taskType.GenericTypeArguments.Length > 0)
            {
                return taskType.GetProperty("Result")?.GetValue(task, null);
            }

            return null;
        }

        return handler.Method.Invoke(handler.Target, parameters);
    }

    internal virtual VerbMethod PickHandler()
    {
        var allCandidates = from m in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            where m.Name == this.Verb || m.Name == this.Verb + "Async"
                            let numParameters = m.GetParameters().Length
                            let returnsTask = IsAsync(m)
                            orderby numParameters descending, returnsTask descending
                            let verb = new VerbMethod(m, this, returnsTask)
                            group verb by numParameters into g
                            select g;


        if (allCandidates.Any() == false)
        {
            throw new MissingVerbException(this.Verb);
        }

        var highestGroup = allCandidates.First().ToArray();
        var numAsync = highestGroup.Count(x => x.IsAsync);

        if (
            (numAsync > 1) ||
            (highestGroup.Length == 2 && numAsync == 0) ||
            (highestGroup.Length > 2 && numAsync != 1))
        {
            throw new AmbiguousVerbException(this.Verb);
        }

        return highestGroup.First();
    }
}
