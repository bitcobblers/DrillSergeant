using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace DrillSergeant;

public class Behavior : IBehavior
{
    protected readonly List<IStep> steps = new();

    public Behavior(object input)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
        dynamic castedInput = new ExpandoObject();
        var dict = (IDictionary<string, object?>)castedInput;

        foreach(var property in input.GetType().GetProperties(flags))
        {
            dict[property.Name] = property.GetValue(input);
        }

        this.Input = dict;
    }

    public IDictionary<string,object?> Context { get; } = new ExpandoObject();

    public IDictionary<string, object?> Input { get; }

    public bool LogContext { get; private set; }

    public Behavior AddStep(IStep step)
    {
        this.steps.Add(step);
        return this;
    }

    public Behavior EnableContextLogging()
    {
        this.LogContext = true;
        return this;
    }

    public IEnumerator<IStep> GetEnumerator() => steps.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
