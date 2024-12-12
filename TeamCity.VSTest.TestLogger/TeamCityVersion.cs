// ReSharper disable InvertIf
// ReSharper disable MemberCanBePrivate.Global
namespace TeamCity.VSTest.TestLogger;

using System;
using System.Text.RegularExpressions;

public class TeamCityVersion : IComparable<TeamCityVersion>
{
    private static readonly Regex VersionRegex = new(@"^(?<Major>\d+)(\.(?<Minor>\d+))?(\.(?<Patch>\d+))?", RegexOptions.Compiled);

    public TeamCityVersion(string? version)
    {
        try
        {
            if (string.IsNullOrEmpty(version)) return;
            
            var match = VersionRegex.Match(version);
            if (!match.Success) return;
            
            var majorGroup = match.Groups["Major"];
            if (int.TryParse(majorGroup.Value, out var val))
            {
                Major = val;
            }

            var minorGroup = match.Groups["Minor"];
            if (minorGroup.Success && int.TryParse(minorGroup.Value, out val))
            {
                Minor = val;
            }

            var patchGroup = match.Groups["Patch"];
            if (patchGroup.Success && int.TryParse(patchGroup.Value, out val))
            {
                Patch = val;
            }
        }
        catch
        {
            // ignored
        }
    }

    public int Major { get; }

    public int Minor { get; }

    public int Patch { get; }

    public int CompareTo(TeamCityVersion other)
    {
        var result = Major.CompareTo(other.Major);
        if (result != 0)
            return result;

        result = Minor.CompareTo(other.Minor);
        if (result != 0)
            return result;

        return Patch.CompareTo(other.Patch);
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }
}