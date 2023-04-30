using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace JustBehave.Core;

public class BehaviorDiscoverer : TheoryDiscoverer
{
    public BehaviorDiscoverer(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
        // Uncomment this line to debug behavior discovery.
        System.Diagnostics.Debugger.Launch();
    }

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute, object[] dataRow)
    {
        return new[] 
        { 
            new BehaviorTestCase(
                DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod,
                dataRow)
        };
    }

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
    {
        //return base.CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
        return new[]
        {
            new BehaviorTheoryTestCase(
                DiagnosticMessageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod)
        };
    }
}
