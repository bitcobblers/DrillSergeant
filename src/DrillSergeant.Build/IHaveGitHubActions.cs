using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IHaveGitHubActions : INukeBuild
{
    GitHubActions? GitHubActions => GitHubActions.Instance;
}