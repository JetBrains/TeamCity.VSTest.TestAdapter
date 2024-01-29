// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.VSTest.TestLogger;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal class Options : IOptions
{
    private static readonly Dictionary<string, string> Envs = new(StringComparer.OrdinalIgnoreCase);
    private static readonly TeamCityVersion TestMetadataSupportVersionVal = new("2018.2");
    private static readonly TeamCityVersion VersionVal;
    private static readonly string RootFlowIdVal;
    private static readonly string ServiceMessagesFileSavePathVal;
    private static readonly bool FallbackToStdOutTestReportingVal;
    private static readonly bool AllowServiceMessageBackupVal;
    private static readonly bool AllowExperimentalVal;
    private static readonly string ServiceMessagesBackupSourceVal;
    private static readonly string ServiceMessagesBackupPathVal;
    private static readonly bool MetadataEnableVal;
    private static readonly string FailedTestsReportSavePathVal;

    static Options()
    {
        foreach (var entry in Environment.GetEnvironmentVariables().OfType<DictionaryEntry>())
        {
            var key = entry.Key?.ToString().Trim() ?? string.Empty;
            var value = entry.Value?.ToString().Trim() ?? string.Empty; 
            Envs[key] = value;
        }
            
        VersionVal = new TeamCityVersion(GetEnvironmentVariable("TEAMCITY_VERSION"));
        RootFlowIdVal = GetEnvironmentVariable("TEAMCITY_PROCESS_FLOW_ID");
        ServiceMessagesFileSavePathVal = GetEnvironmentVariable("TEAMCITY_TEST_REPORT_FILES_PATH");
        FallbackToStdOutTestReportingVal = string.IsNullOrEmpty(ServiceMessagesFileSavePathVal) || GetBool(GetEnvironmentVariable("TEAMCITY_FALLBACK_TO_STDOUT_TEST_REPORTING"), false);
        ServiceMessagesBackupPathVal = GetEnvironmentVariable("TEAMCITY_SERVICE_MESSAGES_PATH");
        AllowServiceMessageBackupVal = !string.IsNullOrEmpty(ServiceMessagesBackupPathVal);
        ServiceMessagesBackupSourceVal = Guid.NewGuid().ToString().Substring(0, 8);
        if (AllowServiceMessageBackupVal)
        {
            try
            {
                if (!Directory.Exists(ServiceMessagesBackupPathVal))
                {
                    Directory.CreateDirectory(ServiceMessagesBackupPathVal);
                }
            }
            catch
            {
                AllowServiceMessageBackupVal = false;
            }
        }
            
        AllowExperimentalVal = GetBool(GetEnvironmentVariable("TEAMCITY_LOGGER_ALLOW_EXPERIMENTAL"), true);
        MetadataEnableVal = GetBool(GetEnvironmentVariable("TEAMCITY_DOTNET_TEST_METADATA_ENABLE"), true);
        FailedTestsReportSavePathVal = GetEnvironmentVariable("TEAMCITY_FAILED_TESTS_REPORTING_PATH");
            
        Environment.SetEnvironmentVariable("TEAMCITY_PROJECT_NAME", string.Empty);
    }

    public string? TestRunDirectory { get; set; }
        
    public TeamCityVersion Version => VersionVal;

    public string RootFlowId => RootFlowIdVal;

    public string ServiceMessagesFileSavePath => ServiceMessagesFileSavePathVal;

    public bool FallbackToStdOutTestReporting => FallbackToStdOutTestReportingVal;

    public bool AllowServiceMessageBackup => AllowServiceMessageBackupVal;

    public string ServiceMessagesBackupPath => ServiceMessagesBackupPathVal;

    public string ServiceMessagesBackupSource => ServiceMessagesBackupSourceVal;

    public bool AllowExperimental => AllowExperimentalVal;

    public bool MetadataEnable => MetadataEnableVal;
    
    public string FailedTestsReportSavePath => FailedTestsReportSavePathVal;

    public TeamCityVersion TestMetadataSupportVersion => TestMetadataSupportVersionVal;

    private static string GetEnvironmentVariable(string name) => Envs.TryGetValue(name, out var val) ? val : string.Empty;

    private static bool GetBool(string value, bool defaultValue) => !string.IsNullOrEmpty(value) && bool.TryParse(value, out var result) ? result : defaultValue;
}