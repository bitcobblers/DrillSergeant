using System;
using System.Linq;
using System.Reflection;

namespace DrillSergeant;

public class VerbStep<TInput> : BaseStep
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

    protected override Delegate PickHandler()
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

        if (highestGroup.Length >= 2 && numAsync != 1)
        {
            throw new AmbiguousVerbException(this.Verb);
        }

        // Prefer async.
        var handler = highestGroup.FirstOrDefault(x => x.IsAsync) ?? highestGroup.First();

        return handler.Method.ToDelegate(handler.Target);
    }
}