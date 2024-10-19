using System.Text.RegularExpressions;
using FcadHackProxy.FilteringSettings;
using Newtonsoft.Json.Linq;

namespace FcadHackProxy.Services;

public class MessageFilterService(FilterSettings filterSettings)
{
    private bool _isMessageContainsSensitiveData = false;
    private FilterSettings _filterSettings = filterSettings;
    public async Task<JObject> ExecuteAsync(JObject jsonObject)
    {
        var filteredMessage = await FilterMessage(jsonObject);
        return filteredMessage;
    }
    
    private async Task<JObject> FilterMessage(JObject jsonMessage)
    {
        foreach (var personalDataField in Constants.PersonalDataFields)
        {
            if (jsonMessage.ContainsKey(personalDataField))
            {
                _isMessageContainsSensitiveData = true;
                if (_filterSettings.RemoveSensitiveFields)
                {
                    jsonMessage.Remove(personalDataField);
                }

                if (_filterSettings.MaskSensitiveMessages)
                {
                    jsonMessage[personalDataField] = "*";
                }
            }
        }

        var messageField = jsonMessage["Message"]!.ToString();
        var filteredMessageField = FilterAllSensitiveData(messageField, _filterSettings.RegexPatterns);

        jsonMessage["Message"] = filteredMessageField;
        return jsonMessage;
    }
    
    private string Filter(string message, string pattern)
    {
        if (_filterSettings.RemoveSensitiveFields)
        {
            return Regex.Replace(message, pattern, "");
        }
        else
        {
            return Regex.Replace(message, pattern, _filterSettings.MaskingSymbol);
        }
    }

    private string FilterAllSensitiveData(string message, List<string> regexPatterns)
    {
        foreach (var pattern in regexPatterns)
        {
            message = Filter(message, pattern);
        }

        return message;
    }
    
}