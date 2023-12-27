namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Text.RegularExpressions;

    public class TeamCityVersion: IComparable<TeamCityVersion>
    {
        private static readonly Regex VersionRegex = new Regex("^(\\d+)(\\.(\\d+)|).*", RegexOptions.Compiled);

        public TeamCityVersion(string? version)
        {
            try
            {
                if (!string.IsNullOrEmpty(version))
                {
                    var match = VersionRegex.Match(version);
                    if (match.Success)
                    {
                        if (int.TryParse(match.Groups[1].Value, out var val))
                        {
                            Major = val;
                        }

                        if (int.TryParse(match.Groups[3].Value, out val))
                        {
                            Minor = val;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public int Major { get; private set; }

        public int Minor { get; private set; }

        public int CompareTo(TeamCityVersion other)
        {
            var result = Major.CompareTo(other.Major);
            if (result != 0)
            {
                return result;
            }

            return Minor.CompareTo(other.Minor);
        }

        public override string ToString()
        {
            return Major + "." + Minor;
        }
    }
}