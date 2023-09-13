using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.ProjectModel;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IHaveSolution : INukeBuild
{
    [Solution][Required] Solution Solution => TryGetValue(() => Solution)!;
}