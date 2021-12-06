// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToReturnStatement
namespace TeamCity.VSTest.TestLogger
{
    using System;

    internal class TestNameProvider : ITestNameProvider
    {
        public string GetTestName(string fullyQualifiedName, string displayName)
        {
            fullyQualifiedName = fullyQualifiedName?.Trim() ?? string.Empty;
            displayName = displayName?.Trim() ?? string.Empty;

            if (fullyQualifiedName == string.Empty)
            {
                return displayName;
            }
            
            if (displayName == string.Empty)
            {
                return fullyQualifiedName;
            }

            if (!fullyQualifiedName.Contains(displayName))
            {
                return fullyQualifiedName + GetArgs(displayName);
            }

            return fullyQualifiedName.Length > displayName.Length ? fullyQualifiedName : displayName;
        }

        private static string GetArgs(string name)
        {
            if (!name.TrimEnd().EndsWith(")"))
            {
                return string.Empty;
            }

            var argsPosition = name.IndexOf("(", StringComparison.Ordinal);
            if (argsPosition < 0)
            {
                return string.Empty;
            }

            return name.Substring(argsPosition, name.Length - argsPosition);
        }
    }
}