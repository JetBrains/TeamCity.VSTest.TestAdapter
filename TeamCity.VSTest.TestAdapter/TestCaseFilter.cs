namespace TeamCity.VSTest.TestAdapter
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal class TestCaseFilter : ITestCaseFilter
    {
        private static readonly HashSet<Uri> NotSupportedExecutorUri = new HashSet<Uri>
        {
            new Uri("executor://xunit/VsTestRunner"),
            new Uri("executor://xunit/VsTestRunner2")
        };
        
        public bool IsSupported(TestCase testCase)
        {
            if (testCase == null) throw new ArgumentNullException(nameof(testCase));
            return !NotSupportedExecutorUri.Contains(testCase.ExecutorUri);
        }
    }
}
