﻿using System;
using System.Linq;
using System.Reflection;

namespace DrillSergeant;

/// <summary>
/// Defines a step that is defined as a class instance.
/// </summary>
public class VerbStep : BaseStep
{
    public record VerbMethod(MethodInfo Method, object Target, bool IsAsync);

    /// <summary>
    /// Initializes a new instance of the <see cref="VerbStep"/> class.
    /// </summary>
    public VerbStep()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VerbStep"/> class.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    public VerbStep(string? name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? GetType().Name : name.Trim();
    }

    /// <inheritdoc />
    protected override Delegate PickHandler()
    {
        var allCandidates = from m in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            where m.Name == Verb || m.Name == Verb + "Async"
                            let numParameters = m.GetParameters().Length
                            let returnsTask = IsAsync(m)
                            orderby numParameters descending, returnsTask descending
                            let verb = new VerbMethod(m, this, returnsTask)
                            group verb by numParameters into g
                            select g;


        if (allCandidates.Any() == false)
        {
            throw new MissingVerbHandlerException(Verb);
        }

        var highestGroup = allCandidates.First().ToArray();
        var numAsync = highestGroup.Count(x => x.IsAsync);

        if (highestGroup.Length >= 2 && numAsync != 1)
        {
            throw new AmbiguousVerbHandlerException(Verb);
        }

        // Prefer async.
        var handler = highestGroup.FirstOrDefault(x => x.IsAsync) ?? highestGroup.First();

        return handler.Method.ToDelegate(handler.Target);
    }
}