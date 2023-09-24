using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.ProjectModel;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IHaveSolution : INukeBuild
{
    [Required, Solution] Solution Solution => 
        TryGetValue(() => Solution)!;
}
