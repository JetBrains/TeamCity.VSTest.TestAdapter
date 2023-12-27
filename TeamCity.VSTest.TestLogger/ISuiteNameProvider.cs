namespace TeamCity.VSTest.TestLogger
{
    internal interface ISuiteNameProvider
    {
        string GetSuiteName(string? source);
    }
}