namespace TeamCity.VSTest.TestLogger
{
    internal static class Strings
    {
        public static bool IsNullOrWhiteSpace(string? str)
            => str == null || str.Trim() == string.Empty;
    }
}
