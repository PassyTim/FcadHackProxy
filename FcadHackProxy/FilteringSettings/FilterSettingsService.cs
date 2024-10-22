using Newtonsoft.Json;
using StackExchange.Redis;

namespace FcadHackProxy.FilteringSettings;

public class FilterSettingsService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private ISubscriber _subscriber;
    private readonly string _settingsKey = "filter_settings";
    private readonly string _channelName = "settings_channel";
    public FilterSettings CurrentSettings { get; private set; }

    public FilterSettingsService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = _redis.GetDatabase();
        _subscriber = _redis.GetSubscriber();
        
        LoadSettings();
        
        _subscriber.Subscribe(_channelName, (channel, message) =>
        {
            LoadSettings(); 
        });
    }

    private void LoadSettings()
    {
        var settingsJson = _database.StringGet(_settingsKey);
        if (!string.IsNullOrEmpty(settingsJson))
        {
            CurrentSettings = JsonConvert.DeserializeObject<FilterSettings>(settingsJson);
        }
        else
        {
            CurrentSettings = new FilterSettings();
        }
    }
    
    public void UpdateSettings(FilterSettings newSettings)
    {
        var settingsJson = JsonConvert.SerializeObject(newSettings);
        _database.StringSet(_settingsKey, settingsJson);

        _subscriber.Publish(_channelName, "SettingsUpdated");
    }
    
    public void AddRegexPattern(string name, string pattern)
    {
        if (!CurrentSettings.AdditionalRegexPatterns.Any(p => p.Contains(pattern)))
        {
            CurrentSettings.AdditionalRegexPatterns.Add($"{name}:{pattern}");
            UpdateSettings(CurrentSettings);
        }
    }

    public void RemoveRegexPattern(string name)
    {
        var patternToRemove = CurrentSettings.AdditionalRegexPatterns.FirstOrDefault(p => p.StartsWith($"{name}:"));
        if (patternToRemove != null)
        {
            CurrentSettings.AdditionalRegexPatterns.Remove(patternToRemove);
            UpdateSettings(CurrentSettings);
        }
    }

    public List<string> GetAllPatterns()
    {
        return CurrentSettings.AdditionalRegexPatterns;
    }
}