namespace TeamCity.VSTest.TestLogger
{
    internal interface ISuiteNameProvider
    {
        [NotNull]
        string GetSuiteName([CanBeNull] string baseDirectory, [CanBeNull] string source);

        void Reset();
    }
}