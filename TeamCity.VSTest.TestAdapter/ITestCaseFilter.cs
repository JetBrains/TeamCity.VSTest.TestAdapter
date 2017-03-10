using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace TeamCity.VSTest.TestAdapter
{
    internal interface ITestCaseFilter
    {
        bool IsSupported([NotNull] TestCase testCase);
    }
}