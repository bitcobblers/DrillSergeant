using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrillSergeant;

public interface IStep : IDisposable
{
    string Verb { get; }
    
    string Name { get; }

    Task Execute(IDictionary<string,object?> context, object input);
}
