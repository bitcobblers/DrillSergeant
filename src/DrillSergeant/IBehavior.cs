using System.Collections.Generic;

namespace DrillSergeant;

public interface IBehavior : IEnumerable<IStep>
{
    object Context { get; }
    object Input { get; }
}
