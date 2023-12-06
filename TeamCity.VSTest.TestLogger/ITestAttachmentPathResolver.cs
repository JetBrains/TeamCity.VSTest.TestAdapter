namespace TeamCity.VSTest.TestLogger
{
    internal interface ITestAttachmentPathResolver
    {
        [NotNull]
        public string Resolve([NotNull] string testFullyQualifiedName);
    }
}