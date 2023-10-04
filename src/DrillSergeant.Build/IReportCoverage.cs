using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Codecov;
using Nuke.Common.Tools.ReportGenerator;
// ReSharper disable AllUnderscoreLocalParameterName
// ReSharper disable VariableHidesOuterVariable

namespace DrillSergeant.Build;

[PublicAPI]
public interface IReportCoverage : ITest, IHaveReports, IHaveGitRepository
{
    [Secret, Parameter] string? CodecovToken => TryGetValue(() => CodecovToken);

    AbsolutePath CoverageReportDirectory => ReportDirectory / "coverage-report";
    AbsolutePath CoverageReportArchive => CoverageReportDirectory.WithExtension("zip");

    Target ReportCoverage => _ => _
        .DependsOn(Test)
        .TryAfter<ITest>()
        .Consumes(Test)
        .Produces(CoverageReportArchive)
        .Requires(() => CodecovToken != null)
        .Executes(() =>
        {
            CodecovTasks.Codecov(_ => _
                .Apply(CodecovSettings));

            ReportGeneratorTasks.ReportGenerator(_ => _
                .Apply(ReportGeneratorSettings));

            CoverageReportDirectory.ZipTo(CoverageReportArchive, fileMode: FileMode.Create);
        });

    Configure<CodecovSettings> CodecovSettings => _ => _
        .SetFiles(TestResultsDirectory.GlobFiles("*.xml").Select(x => x.ToString()))
        .SetToken(CodecovToken)
        .SetBranch(GitRepository.Branch)
        .SetSha(GitRepository.Commit);

    Configure<ReportGeneratorSettings> ReportGeneratorSettings => _ => _
        .SetReports(TestResultsDirectory / "*.xml")
        .SetReportTypes(ReportTypes.HtmlInline)
        .SetTargetDirectory(CoverageReportDirectory);
}
