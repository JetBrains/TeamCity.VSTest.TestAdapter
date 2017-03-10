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

        private readonly IEnvironmentInfo _environmentInfo;

        public TestCaseFilter([NotNull] IEnvironmentInfo environmentInfo)
        {
            if (environmentInfo == null) throw new ArgumentNullException(nameof(environmentInfo));
            _environmentInfo = environmentInfo;
        }

        public bool IsSupported(TestCase testCase)
        {
            if (testCase == null) throw new ArgumentNullException(nameof(testCase));
            if (!_environmentInfo.IsUnderTeamCity)
                return true;

            return !NotSupportedExecutorUri.Contains(testCase.ExecutorUri);
        }
    }
}