using System;
using System.Collections.Generic;
using System.Linq;

namespace JustBehave;

public class Behavior
{
    private readonly Step[] _steps;
    private readonly Type contextType;
    private readonly Type inputType;

    public Behavior(IEnumerable<Step> steps, Type contextType, Type inputType)
    {
        _steps = steps.ToArray();
        this.contextType = contextType;
        this.inputType = inputType;
    }

    public IEnumerable<Step> Steps => _steps;
    public Type ContextType => this.contextType;
    public Type InputType => this.inputType;
}