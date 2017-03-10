namespace TeamCity.VSTest.TestAdapter
{
    internal interface ISuiteNameProvider
    {
        [NotNull]
        string GetSuiteName([CanBeNull] string baseDirectory, [CanBeNull] string source);

        void Reset();
    }
}