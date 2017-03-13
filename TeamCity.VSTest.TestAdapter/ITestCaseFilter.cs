namespace TeamCity.VSTest.TestAdapter
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    internal interface ITestCaseFilter
    {
        void RegisterOutputMessage([NotNull] string outputLine);

        bool IsSupported([NotNull] TestCase testCase);
    }
}