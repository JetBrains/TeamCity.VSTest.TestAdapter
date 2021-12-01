// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InvertIf
namespace TeamCity.VSTest.TestLogger
{
    using System;

    internal class TestNameFactory : ITestNameFactory
    {
        public string Create(string fullyQualifiedName, [CanBeNull]string displayName)
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
                var parts = fullyQualifiedName.Split('.');
                if (parts.Length > 0)
                {
                    var lastPart = parts[parts.Length - 1];
                    if (lastPart.Length > 0 && displayName.StartsWith(lastPart) && displayName.Length > lastPart.Length + 1)
                    {
                        parts[parts.Length - 1] = lastPart + displayName.Substring(lastPart.Length, displayName.Length - lastPart.Length).Trim();
                        return string.Join(".", parts);
                    }
                }
            }

            return fullyQualifiedName.Length > displayName.Length ? fullyQualifiedName : displayName;
        }
    }
}