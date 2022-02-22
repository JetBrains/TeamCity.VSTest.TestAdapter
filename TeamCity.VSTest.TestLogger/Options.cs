// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class Options : IOptions
    {
        private static readonly Dictionary<string, string> Envs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly TeamCityVersion TestMetadataSupportVersionVal = new TeamCityVersion("2018.2");
        private static readonly TeamCityVersion VersionVal;
        private static readonly string RootFlowIdVal;
        private static readonly bool AllowServiceMessageBackupVal;
        private static readonly bool AllowExperimentalVal;
        private static readonly string ServiceMessagesSourceVal;
        private static readonly string ServiceMessagesPathVal;
        private static readonly bool MetadataEnableVal;

        static Options()
        {
            foreach (var entry in Environment.GetEnvironmentVariables().OfType<DictionaryEntry>())
            {
                var key = entry.Key?.ToString()?.Trim() ?? string.Empty;
                var value = entry.Value?.ToString()?.Trim() ?? string.Empty; 
                Envs[key] = value;
            }
            
            VersionVal = new TeamCityVersion(GetEnvironmentVariable("TEAMCITY_VERSION"));
            RootFlowIdVal = GetEnvironmentVariable("TEAMCITY_PROCESS_FLOW_ID") ?? string.Empty;
            
            ServiceMessagesPathVal = GetEnvironmentVariable("TEAMCITY_SERVICE_MESSAGES_PATH") ?? string.Empty;
            AllowServiceMessageBackupVal = !string.IsNullOrEmpty(ServiceMessagesPathVal);
            ServiceMessagesSourceVal = Guid.NewGuid().ToString().Substring(0, 8);
            if (AllowServiceMessageBackupVal)
            {
                try
                {
                    if (!Directory.Exists(ServiceMessagesPathVal))
                    {
                        Directory.CreateDirectory(ServiceMessagesPathVal);
                    }
                }
                catch
                {
                    AllowServiceMessageBackupVal = false;
                }
            }
            
            AllowExperimentalVal = GetBool(GetEnvironmentVariable("TEAMCITY_LOGGER_ALLOW_EXPERIMENTAL"), true);
            MetadataEnableVal = GetBool(GetEnvironmentVariable("TEAMCITY_DOTNET_TEST_METADATA_ENABLE"), true);
            
            Environment.SetEnvironmentVariable("TEAMCITY_PROJECT_NAME", string.Empty);
        }

        public string TestRunDirectory { get; set; }
        
        public TeamCityVersion Version => VersionVal;

        public string RootFlowId => RootFlowIdVal;

        public bool AllowServiceMessageBackup => AllowServiceMessageBackupVal;

        public string ServiceMessagesSource => ServiceMessagesSourceVal;

        private string ServiceMessagesPath => AllowServiceMessageBackupVal ? Path.Combine(ServiceMessagesPathVal, ServiceMessagesSource) : ServiceMessagesSource;

        public string ServiceIndicesFile => ServiceMessagesPath;

        public string ServiceMessagesFile => ServiceMessagesPath + ".msg";

        public bool AllowExperimental => AllowExperimentalVal;

        public bool MetadataEnable => MetadataEnableVal;

        public TeamCityVersion TestMetadataSupportVersion => TestMetadataSupportVersionVal;

        private static string GetEnvironmentVariable(string name) => Envs.TryGetValue(name, out var val) ? val : string.Empty;

        private static bool GetBool(string value, bool defaultValue) => !string.IsNullOrEmpty(value) && bool.TryParse(value, out var result) ? result : defaultValue;
    }
}
