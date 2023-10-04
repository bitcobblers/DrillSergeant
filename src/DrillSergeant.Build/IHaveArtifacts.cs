using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IHaveArtifacts : INukeBuild
{
    AbsolutePath ArtifactDirectory => RootDirectory / "artifacts";
}
