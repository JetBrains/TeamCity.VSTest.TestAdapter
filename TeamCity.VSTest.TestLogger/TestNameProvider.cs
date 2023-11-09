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

            if (!string.IsNullOrEmpty(displayName)) // TODO check env variable with feature toggle
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
            var typedArgsPosition = name.IndexOf("<", StringComparison.Ordinal);
            var hasTypes = typedArgsPosition >= 0;
            var argsPosition = name.IndexOf("(", StringComparison.Ordinal);
            var hasArgs = argsPosition >= 0;
            if (!hasArgs && !hasTypes)
            {
                return string.Empty;
            }

            if (!hasArgs || hasTypes && typedArgsPosition < argsPosition)
            {
                argsPosition = typedArgsPosition;
            }

            name = name.Substring(argsPosition, name.Length - argsPosition).Trim();
            if (hasArgs && !name.EndsWith(")"))
            {
                name += ")";
            }
            
            return name;
        }
    }
}