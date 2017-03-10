namespace TeamCity.VSTest.TestAdapter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class SuiteNameProviderTests
    {
        public const string Null = "null";
        public const string Br = ";";

        private static SuiteNameProvider CreateInstance()
        {
            return new SuiteNameProvider();
        }

        private class GetSuiteNameAction
        {
            private GetSuiteNameAction(
                [CanBeNull] string baseDirectory,
                [CanBeNull] string source,
                string expectedSuiteName)
            {
                BaseDirectory = baseDirectory;
                Source = source;
                ExpectedSuiteName = expectedSuiteName;
            }

            public string BaseDirectory { [CanBeNull] get; }

            public string Source { [CanBeNull] get; }

            public string ExpectedSuiteName { [CanBeNull] get; }

            private static GetSuiteNameAction Create(string description)
            {
                var parts = description.Split(new[] {Br}, StringSplitOptions.None);
                return new GetSuiteNameAction(ParsePart(parts, 0), ParsePart(parts, 1), ParsePart(parts, 2));
            }

            public static IEnumerable<GetSuiteNameAction> CreateMany(string descriptions)
            {
                var lines = descriptions.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                return
                    from line in lines
                    select Create(line);
            }

            private static string ParsePart(string[] parts, int index)
            {
                if (index >= parts.Length)
                    return null;

                var value = parts[index];
                if (Null.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                    return null;

                return value;
            }
        }

        [Test]
        [TestCase(@"c:\dir" + Br + @"c:\dir\abc.dll" + Br + @"abc.dll")]
        [TestCase(@"c:\dir" + Br + @"somePath\abc.dll" + Br + @"abc.dll")]
        [TestCase(@"c:\dir" + Br + @"abc.dll" + Br + @"abc.dll")]
        [TestCase(Null + Br + @"c:\dir\abc.dll" + Br + @"abc.dll")]
        [TestCase(Null + Br + @"somePath\abc.dll" + Br + @"abc.dll")]
        [TestCase(Null + Br + @"abc.dll" + Br + @"abc.dll")]
        [TestCase(Null + Br + Null + Br + SuiteNameProvider.DefaultSuiteName)]
        [TestCase("" + Br + "" + Br + SuiteNameProvider.DefaultSuiteName)]
        public void ShouldProvideSuiteName(string descriptions)
        {
            // Given
            var nameProvider = CreateInstance();

            // When
            foreach (var getSuiteNameAction in GetSuiteNameAction.CreateMany(descriptions))
            {
                var actualSuiteName = nameProvider.GetSuiteName(getSuiteNameAction.BaseDirectory, getSuiteNameAction.Source);

                // Then
                actualSuiteName.ShouldBe(getSuiteNameAction.ExpectedSuiteName);
            }
        }
    }
}