using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IHaveGitRepository : INukeBuild
{
    [Required, GitRepository] GitRepository GitRepository => TryGetValue(() => GitRepository)!;
}