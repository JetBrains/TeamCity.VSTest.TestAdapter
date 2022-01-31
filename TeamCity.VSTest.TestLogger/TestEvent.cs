namespace TeamCity.VSTest.TestLogger
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    public class TestEvent
    {
        public readonly string SuiteName;
        public readonly TestCase TestCase;

        public TestEvent(string suiteName, TestCase testCase)
        {
            SuiteName = suiteName;
            TestCase = testCase;
        }
    }
}