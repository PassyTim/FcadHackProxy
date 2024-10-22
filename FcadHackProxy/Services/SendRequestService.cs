using System.Text;
using Newtonsoft.Json.Linq;

namespace FcadHackProxy.Services;

public class SendRequestService
{
    private readonly HttpClient _httpClient;
    public SendRequestService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task SendPostRequestAsync(JObject jsonObject)
    {
        var endpoint = jsonObject["Endpoint"]?.ToString();
        
        jsonObject.Remove("Endpoint");

        var jsonContent = jsonObject.ToString();

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        else
        {   
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Ошибка при отправке запроса: {response.StatusCode}, {errorContent}");
        }
    }
}