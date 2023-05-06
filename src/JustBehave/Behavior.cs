using System;
using System.Collections.Generic;
using System.Linq;

namespace JustBehave;

public class Behavior
{
    private readonly IStep[] _steps;
    private readonly Type contextType;
    private readonly Type inputType;

    public Behavior(IEnumerable<IStep> steps, Type contextType, Type inputType)
    {
        _steps = steps.ToArray();
        this.contextType = contextType;
        this.inputType = inputType;
    }

    public IEnumerable<IStep> Steps => _steps;
    public Type ContextType => this.contextType;
    public Type InputType => this.inputType;
}