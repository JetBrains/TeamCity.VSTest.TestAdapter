namespace TeamCity.VSTest.TestAdapter
{
    using System;

    internal class EnvironmentInfo: IEnvironmentInfo
    {
        internal const string TeamCityProjectEnvVarName = "TEAMCITY_PROJECT_NAME";

        public EnvironmentInfo()
        {
            var teamCityProjectName = Environment.GetEnvironmentVariable(TeamCityProjectEnvVarName);
            IsUnderTeamCity = !string.IsNullOrEmpty(teamCityProjectName);
        }

        public bool IsUnderTeamCity { get; }
    }
}
