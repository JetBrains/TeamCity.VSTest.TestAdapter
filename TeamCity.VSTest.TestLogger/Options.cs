namespace TeamCity.VSTest.TestLogger
{
    using System;

    internal class Options : IOptions
    {
        private static readonly TeamCityVersion VersionVal = new TeamCityVersion(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));
        private static readonly string RootFlowIdVal = Environment.GetEnvironmentVariable("TEAMCITY_PROCESS_FLOW_ID");
        private static readonly bool AllowExperimentalVal = GetBool(Environment.GetEnvironmentVariable("TEAMCITY_LOGGER_ALLOW_EXPERIMENTAL"), true);
        private static readonly bool MetadataEnableVal = GetBool(Environment.GetEnvironmentVariable("TEAMCITY_DOTNET_TEST_METADATA_ENABLE"), true);
        private static readonly TeamCityVersion TestMetadataSupportVersionVal = new TeamCityVersion("2018.2");

        public string TestRunDirectory { get; set; }
        
        public TeamCityVersion Version => VersionVal;

        public string RootFlowId => RootFlowIdVal;

        public bool AllowExperimental => AllowExperimentalVal;

        public bool MetadataEnable => MetadataEnableVal;

        public TeamCityVersion TestMetadataSupportVersion => TestMetadataSupportVersionVal;

        private static bool GetBool(string value, bool defaultValue) => !string.IsNullOrEmpty(value) && bool.TryParse(value, out var result) ? result : defaultValue;
    }
}
