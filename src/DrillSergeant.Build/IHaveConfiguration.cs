using JetBrains.Annotations;
using Nuke.Common;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IHaveConfiguration : INukeBuild
{
    [Parameter] Configuration Configuration => TryGetValue(() => Configuration) ?? (IsLocalBuild ? Configuration.Debug : Configuration.Release);
}