namespace TeamCity.VSTest.TestLogger
{
    using System.IO;

    internal class SuiteNameProvider : ISuiteNameProvider
    {
        public string GetSuiteName(string source)
        {
            try
            {
                return Strings.IsNullOrWhiteSpace(source) ? string.Empty : Path.GetFileNameWithoutExtension(source);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}