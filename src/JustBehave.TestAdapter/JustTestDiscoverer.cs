using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.ComponentModel;

namespace JustBehave.TestAdapter;

[FileExtension(".dll")]
[FileExtension(".exe")]
[Category("managed")]
[DefaultExecutorUri(Constants.ExecutorUri)]
public class JustTestDiscoverer : ITestDiscoverer
{
    public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
    {
        logger.SendMessage(TestMessageLevel.Warning, "JustBehave test discoverer not implemented.");
    }
}
