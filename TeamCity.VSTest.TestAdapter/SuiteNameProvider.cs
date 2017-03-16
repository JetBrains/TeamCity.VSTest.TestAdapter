namespace TeamCity.VSTest.TestAdapter
{
    using System.IO;

    internal class SuiteNameProvider : ISuiteNameProvider
    {
        internal const string DefaultSuiteName = "Tests";

        public string GetSuiteName(string baseDirectory, string source)
        {
            if (source.IsNullOrWhiteSpace())
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