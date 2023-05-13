﻿using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Core;

public class BehaviorDiscoverer : TheoryDiscoverer
{
    public BehaviorDiscoverer(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
        // Uncomment this line to debug behavior discovery.
        // System.Diagnostics.Debugger.Launch();
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
}