using System;
using System.Linq;
using System.Reflection;

namespace DrillSergeant;

/// <summary>
/// Defines a step that is defined as a class instance.
/// </summary>
public class VerbStep : BaseStep
{
    private record VerbMethod(MethodInfo Method, object Target, bool IsAsync);

    /// <summary>
    /// Initializes a new instance of the <see cref="VerbStep"/> class.
    /// </summary>
    protected VerbStep()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VerbStep"/> class.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    protected VerbStep(string? name) =>
        Name = string.IsNullOrWhiteSpace(name) ? GetType().Name : name.Trim();

    public override string Name { get; }

    /// <inheritdoc />
    protected override Delegate PickHandler()
    {
        var allCandidates = (from m in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                             where m.Name == Verb || m.Name == Verb + "Async"
                             let numParameters = m.GetParameters().Length
                             let returnsTask = IsAsync(m)
                             orderby numParameters descending, returnsTask descending
                             let verb = new VerbMethod(m, this, returnsTask)
                             group verb by numParameters into g
                             select g).ToArray();


        if (allCandidates.Any() == false)
        {
            throw new MissingVerbHandlerException(Verb);
        }

        if (allCandidates.Any(x => x.Key > 0))
        {
            throw new AmbiguousVerbHandlerException(Verb);
        }

        var candidates = allCandidates.First();
        var handler = candidates.FirstOrDefault(x => x.IsAsync) ?? candidates.First();

        return handler.Method.ToDelegate(handler.Target);
    }
}