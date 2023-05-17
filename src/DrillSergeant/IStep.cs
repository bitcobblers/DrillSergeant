using System;
using System.Threading.Tasks;

namespace DrillSergeant;

public interface IStep : IDisposable
{
    string Verb { get; }
    
    string Name { get; }

    Task<object?> Execute(object context, object input, IDependencyResolver resolver);
}
