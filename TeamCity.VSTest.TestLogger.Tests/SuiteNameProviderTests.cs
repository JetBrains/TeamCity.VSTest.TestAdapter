﻿// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace TeamCity.VSTest.TestLogger.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

public class SuiteNameProviderTests
{
    private const string Null = "null";
    private const string Br = ";";

    private static SuiteNameProvider CreateInstance()
    {
        return new SuiteNameProvider();
    }

    private class GetSuiteNameAction
    {
        private GetSuiteNameAction(
            string? baseDirectory,
            string? source,
            string? expectedSuiteName)
        {
            BaseDirectory = baseDirectory;
            Source = source;
            ExpectedSuiteName = expectedSuiteName;
        }

        public string? BaseDirectory { get; }

        public string? Source { get; }

        public string? ExpectedSuiteName { get; }

        private static GetSuiteNameAction Create(string description)
        {
            var parts = description.Split([Br], StringSplitOptions.None);
            return new GetSuiteNameAction(ParsePart(parts, 0), ParsePart(parts, 1), ParsePart(parts, 2));
        }

        public static IEnumerable<GetSuiteNameAction> CreateMany(string descriptions)
        {
            var lines = descriptions.Split([Environment.NewLine], StringSplitOptions.None);
            return
                from line in lines
                select Create(line);
        }

        private static string? ParsePart(IReadOnlyList<string> parts, int index)
        {
            if (index >= parts.Count)
                return null;

            var value = parts[index];
            return Null.Equals(value, StringComparison.CurrentCultureIgnoreCase) ? null : value;
        }
    }

    [Theory]
    [InlineData(@"c:\dir" + Br + @"c:\dir\abc.dll" + Br + "abc")]
    [InlineData(@"c:\dir" + Br + @"somePath\abc.dll" + Br + "abc")]
    [InlineData(@"c:\dir" + Br + "abc.dll" + Br + "abc")]
    [InlineData(Null + Br + @"c:\dir\abc.dll" + Br + "abc")]
    [InlineData(Null + Br + @"somePath\abc.dll" + Br + "abc")]
    [InlineData(Null + Br + "abc.dll" + Br + "abc")]
    [InlineData(Null + Br + Null + Br)]
    [InlineData("" + Br + "" + Br)]
    public void ShouldProvideSuiteName(string descriptions)
    {
        // Given
        var nameProvider = CreateInstance();

        // When
        foreach (var getSuiteNameAction in GetSuiteNameAction.CreateMany(descriptions))
        {
            var actualSuiteName = nameProvider.GetSuiteName(getSuiteNameAction.Source);

            // Then
            actualSuiteName.ShouldBe(getSuiteNameAction.ExpectedSuiteName);
        }
    }
}