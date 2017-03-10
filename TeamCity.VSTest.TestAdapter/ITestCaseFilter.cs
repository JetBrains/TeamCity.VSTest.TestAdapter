namespace TeamCity.VSTest.TestAdapter
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal interface ITestCaseFilter
    {
        bool IsSupported([NotNull] TestCase testCase);
    }
}