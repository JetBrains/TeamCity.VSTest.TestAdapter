namespace TeamCity.VSTest.TestLogger;

internal interface ITestNameProvider
{
    string GetTestName(string? fullyQualifiedName, string? displayName);
}