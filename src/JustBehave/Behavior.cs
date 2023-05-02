using System;
using System.Collections.Generic;
using System.Linq;

namespace JustBehave;

public class Behavior
{
    private readonly Step[] _steps;

    public Behavior(IEnumerable<Step> steps) => _steps = steps.ToArray();

    public IEnumerable<Step> Steps => _steps;
}