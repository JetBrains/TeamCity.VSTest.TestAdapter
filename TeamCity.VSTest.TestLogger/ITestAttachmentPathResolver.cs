namespace TeamCity.VSTest.TestLogger;

internal interface ITestAttachmentPathResolver
{
    public string Resolve(string testFullyQualifiedName);
}