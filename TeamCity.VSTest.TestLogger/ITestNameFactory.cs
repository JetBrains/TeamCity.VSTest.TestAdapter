namespace TeamCity.VSTest.TestLogger
{
    internal interface ITestNameFactory
    {
        [NotNull] string Create([CanBeNull] string fullyQualifiedName, [CanBeNull] string displayName);
    }
}