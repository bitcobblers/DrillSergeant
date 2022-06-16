using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;

namespace JustBehave.TestAdapter
{
    [ExtensionUri(Constants.ExecutorUri)]
    public class JustTestExecutor : ITestExecutor
    {
        public void Cancel()
        {
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Warning, "JustBehave test executor not implemented.");
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Warning, "JustBehave test executor not implemented.");
        }
    }
}