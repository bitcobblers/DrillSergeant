using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tools.GitVersion;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IHaveGitVersion : INukeBuild
{
    [Required, GitVersion]
    GitVersion GitVersion =>
        TryGetValue(() => GitVersion)!;
}
