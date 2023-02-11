using System;
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
        // System.Diagnostics.Debugger.Launch();
    }

    public override IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
    {
        return Array.Empty<IXunitTestCase>();
    }
}