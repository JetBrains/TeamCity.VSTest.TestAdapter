namespace TeamCity.VSTest.TestLogger
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    public class TestEvent
    {
        public readonly string SuiteName;
        public readonly TestCase TestCase;
        public readonly string TestResultDisplayName;

        public TestEvent(string suiteName, TestCase testCase, string testResultDisplayName)
        {
            SuiteName = suiteName;
            TestCase = testCase;
            TestResultDisplayName = testResultDisplayName;
        }
    }
}