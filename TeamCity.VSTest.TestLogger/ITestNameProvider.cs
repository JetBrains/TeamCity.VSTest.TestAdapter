namespace TeamCity.VSTest.TestLogger
{
    internal interface ITestNameProvider
    {
        [NotNull] string GetTestName([CanBeNull] string fullyQualifiedName, [CanBeNull] string displayName);
    }
}