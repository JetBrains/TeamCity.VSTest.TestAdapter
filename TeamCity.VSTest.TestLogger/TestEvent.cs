namespace TeamCity.VSTest.TestLogger;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

public class TestEvent(string suiteName, string displayName, TestCase testCase)
{
    public readonly string SuiteName = suiteName;
    public readonly string DisplayName = displayName;
    public readonly TestCase TestCase = testCase;
}