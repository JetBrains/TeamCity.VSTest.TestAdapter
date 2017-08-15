namespace TeamCity.VSTest.TestLogger
{
    using System.IO;

    internal class SuiteNameProvider : ISuiteNameProvider
    {
        internal const string DefaultSuiteName = "Tests";

        public string GetSuiteName(string baseDirectory, string source)
        {
            if (Strings.IsNullOrWhiteSpace(source))
            {
                return DefaultSuiteName;
            }

            return Path.GetFileName(source);
        }

        public void Reset()
        {
        }
    }
}