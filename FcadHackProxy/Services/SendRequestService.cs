using System.Diagnostics.Metrics;
using System.Text;
using Newtonsoft.Json.Linq;
using Prometheus;

namespace FcadHackProxy.Services;

public class SendRequestService
{
    private readonly HttpClient _httpClient;
    private static readonly Counter _requestCounter = Metrics.CreateCounter("sent_requests_total", "Total number of sent requests");
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
        
        _requestCounter.Inc();

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