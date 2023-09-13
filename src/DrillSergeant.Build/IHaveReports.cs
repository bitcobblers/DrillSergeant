using JetBrains.Annotations;
using Nuke.Common.IO;

namespace DrillSergeant.Build;

[PublicAPI]
public interface IHaveReports : IHaveArtifacts
{
    AbsolutePath ReportDirectory => ArtifactDirectory / "reports";
}