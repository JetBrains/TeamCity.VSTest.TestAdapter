namespace TeamCity.VSTest.TestLogger
{
    internal static class Strings
    {
        public static bool IsNullOrWhiteSpace(string str)
        {
            if (str == null)
            {
                return true;
            }

            return str.Trim() == string.Empty;
        }
    }
}
