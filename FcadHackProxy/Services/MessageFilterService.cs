using System.Text.RegularExpressions;
using FcadHackProxy.Data;
using FcadHackProxy.FilteringSettings;
using Newtonsoft.Json.Linq;

namespace FcadHackProxy.Services;

public class MessageFilterService(
    FilterSettingsService filterSettings,
    IMessageRepository messageRepository,
    SendRequestService sendRequestService)
{
    private bool _isMessageContainsSensitiveData;
    private bool _isSensitiveDataIsOnlyEmailAndLogin;
    private FilterSettings FilterSettings { get; set; } = filterSettings.CurrentSettings;
    public async Task ExecuteAsync(JObject jsonObject)
    {
        var unfilteredJson = new JObject(jsonObject);
        var filteredMessage = await FilterMessage(jsonObject);

        if (_isMessageContainsSensitiveData || (FilterSettings.SaveSensitiveEmail && _isSensitiveDataIsOnlyEmailAndLogin))
        {
            await messageRepository.SaveAsync(unfilteredJson);
        } 
        if (!FilterSettings.BlockSensitiveMessages)
        {
            sendRequestService.SendPostRequestAsync(filteredMessage);
        }
    }
    
    private async Task<JObject> FilterMessage(JObject jsonMessage)
    {
        FilterSensitiveJsonFields(jsonMessage);
        
        var messageField = jsonMessage["Message"]!.ToString();
        var filteredMessageField = FilterAllSensitiveDataInMessageField(messageField, FilterSettings.GetAllRegexPatterns());

        jsonMessage["Message"] = filteredMessageField;
        
        return jsonMessage;
    }
    
    private void FilterSensitiveJsonFields(JObject jsonObject)
    {
        var cleanedJsonObject = new JObject();

        foreach (var property in jsonObject.Properties())
        {
            var cleanedKey = property.Name.Replace("\uFEFF", string.Empty);
            
            JToken cleanedValue;
            if (property.Value.Type == JTokenType.String)
            {
                cleanedValue = property.Value.ToString().Replace("\uFEFF", string.Empty);
            }
            else
            {
                cleanedValue = property.Value;
            }
            
            cleanedJsonObject[cleanedKey] = cleanedValue;
        }
        
        jsonObject.RemoveAll();
        foreach (var cleanedProperty in cleanedJsonObject.Properties())
        {
            jsonObject[cleanedProperty.Name] = cleanedProperty.Value;
        }

        foreach (var personalDataField in Constants.PersonalDataFields)
        {
            if (jsonObject.ContainsKey(personalDataField))
            {
                _isMessageContainsSensitiveData = true;

                if (FilterSettings.RemoveSensitiveFields)
                {
                    jsonObject.Remove(personalDataField);
                }

                if (FilterSettings.MaskSensitiveMessages)
                {
                    jsonObject[personalDataField] = FilterSettings.MaskingSymbols;
                }
            }
        }

        if (!_isMessageContainsSensitiveData && jsonObject.ContainsKey("Email") && jsonObject.ContainsKey("Login"))
        {
            _isSensitiveDataIsOnlyEmailAndLogin = true;
        }
        
        if (jsonObject.ContainsKey("Email") && jsonObject.ContainsKey("Login"))
        {
            if (FilterSettings.RemoveSensitiveFields)
            {
                jsonObject.Remove("Email");
                jsonObject.Remove("Login");
            }

            if (FilterSettings.MaskSensitiveMessages)
            {
                jsonObject["Email"] = FilterSettings.MaskingSymbols;
                jsonObject["Login"] = FilterSettings.MaskingSymbols;
            }
        }
    }

    private string FilterAllSensitiveDataInMessageField(string message, List<string> regexPatterns)
    {
        foreach (var pattern in regexPatterns)
        {
            message = FilterDifferentRegularExpressions(message, pattern);
        }

        return message;
    }
    
    private string FilterDifferentRegularExpressions(string message, string pattern)
    {
        if (FilterSettings.RemoveSensitiveFields)
        {
            if (Regex.Match(message, pattern).Success)
            {
                _isMessageContainsSensitiveData = true;
                _isSensitiveDataIsOnlyEmailAndLogin = false;
            }
            return Regex.Replace(message, pattern, "");
        }

        return Regex.Replace(message, pattern, FilterSettings.MaskingSymbols);
    }
}