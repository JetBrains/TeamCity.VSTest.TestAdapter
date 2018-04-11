namespace TeamCity.VSTest.TestLogger
{
    using System.IO;

    internal class SuiteNameProvider : ISuiteNameProvider
    {
        internal const string DefaultSuiteName = "Tests";

        public string GetSuiteName(string baseDirectory, string source)
            => Strings.IsNullOrWhiteSpace(source) ? DefaultSuiteName : Path.GetFileName(source);

        public void Reset()
        {
        }
    }
}