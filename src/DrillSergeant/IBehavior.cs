using System.Collections.Generic;

namespace DrillSergeant;

public interface IBehavior : IEnumerable<IStep>
{
    IDictionary<string,object?> Context { get; }
    object Input { get; }
    bool LogContext { get; }
}
