using System;

namespace JustBehave;

public interface IStep : IDisposable
{
    string Verb { get; }
    
    string Name { get; }

    object? Execute(object context, object input, IDependencyResolver resolver);
}
