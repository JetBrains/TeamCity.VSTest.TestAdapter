namespace TeamCity.VSTest.TestLogger.Tests;

using Shouldly;
using Xunit;

public class TeamCityVersionTest
{
    [Theory]
    [InlineData("2023.1.1", "2024.1.1", -1)]
    [InlineData("2024.1.1", "2024.2.1", -1)]
    [InlineData("2024.1.1", "2024.1.2", -1)]
    [InlineData("2024.1.1", "2024.1.1", 0)]
    [InlineData("2024.12.1 (build SNAPSHOT)", "2024.12.1 (build SNAPSHOT 2)", 0)]
    [InlineData("2024.12.1 (build SNAPSHOT)", "2024.12.1 (build SNAPSHOT 2)", 0)]
    [InlineData("2024.12.2 (build SNAPSHOT)", "2024.12.1 (build SNAPSHOT 2)", 1)]
    [InlineData("2024.12.1", "2024.12", 1)]
    [InlineData("2024.12.10", "2024.12.2", 1)]
    [InlineData("2018.1", "2018.2", -1)]
    [InlineData("2018.1", "2018.2 (build SNAPSHOT)", -1)]
    [InlineData("2018.2", "2018.2 (build SNAPSHOT)", 0)]
    [InlineData("2018.2 (build SNAPSHOT)", "2018.2 (build SNAPSHOT)", 0)]
    [InlineData("2018.2 (build SNAPSHOT)", "2018.2 (build SNAPSHOT 2)", 0)]
    [InlineData("2018.2 (build SNAPSHOT)", "2018.1", 1)]
    [InlineData("", "2018.2 (build SNAPSHOT)", -1)]
    [InlineData("2018.2 (build SNAPSHOT)", "", 1)]
    [InlineData("2018.2", "2018.2", 0)]
    [InlineData("2018.2.1", "2018.2", 1)]
    [InlineData(null, "2018.2", -1)]
    [InlineData("", "", 0)]
    [InlineData(null, "", 0)]
    [InlineData(null, null, 0)]
    [InlineData("", "abc", 0)]
    [InlineData(".32323", "abc", 0)]
    [InlineData("2018", "2018", 0)]
    [InlineData("2018", "2018.1", -1)]
    [InlineData("10", "11", -1)]
    [InlineData("12", "11", 1)]
    [InlineData("10", "2018.2", -1)]
    public void ShouldCompareTeamCityVersions(string? version1, string? version2, int expectedCompareResult)
    {
        // Given
        var v1 = new TeamCityVersion(version1);
        var v2 = new TeamCityVersion(version2);

        // When
        var actualCompareResult = v1.CompareTo(v2);

        // Then
        actualCompareResult.ShouldBe(expectedCompareResult);
    }
}