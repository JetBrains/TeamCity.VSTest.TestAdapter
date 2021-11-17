namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class Options : IOptions
    {
        private static readonly Dictionary<string, string> Envs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly TeamCityVersion TestMetadataSupportVersionVal = new TeamCityVersion("2018.2");
        private readonly TeamCityVersion _versionVal = new TeamCityVersion(GetEnvironmentVariable("TEAMCITY_VERSION"));
        private readonly string _rootFlowIdVal = GetEnvironmentVariable("TEAMCITY_PROCESS_FLOW_ID");
        private readonly bool _allowExperimentalVal = GetBool(GetEnvironmentVariable("TEAMCITY_LOGGER_ALLOW_EXPERIMENTAL"), true);
        private readonly bool _metadataEnableVal = GetBool(GetEnvironmentVariable("TEAMCITY_DOTNET_TEST_METADATA_ENABLE"), true);

        static Options()
        {
            foreach (var entry in Environment.GetEnvironmentVariables().OfType<DictionaryEntry>())
            {
                var key = entry.Key?.ToString()?.Trim() ?? string.Empty;
                var value = entry.Value?.ToString()?.Trim() ?? string.Empty; 
                Envs[key] = value;
            }
            
            Environment.SetEnvironmentVariable("TEAMCITY_PROJECT_NAME", string.Empty);
        }

        public string TestRunDirectory { get; set; }
        
        public TeamCityVersion Version => _versionVal;

        public string RootFlowId => _rootFlowIdVal;

        public bool AllowExperimental => _allowExperimentalVal;

        public bool MetadataEnable => _metadataEnableVal;

        public TeamCityVersion TestMetadataSupportVersion => TestMetadataSupportVersionVal;

        private static string GetEnvironmentVariable(string name) => Envs.TryGetValue(name, out var val) ? val : string.Empty;

        private static bool GetBool(string value, bool defaultValue) => !string.IsNullOrEmpty(value) && bool.TryParse(value, out var result) ? result : defaultValue;
    }
}
