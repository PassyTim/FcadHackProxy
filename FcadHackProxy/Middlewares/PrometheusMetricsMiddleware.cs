using System.Diagnostics;
using Prometheus;

namespace FcadHackProxy.Middlewares;

public class PrometheusMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Counter RequestCounter = Metrics.CreateCounter("http_requests_total", "Total number of HTTP requests", 
        new CounterConfiguration
        {
            LabelNames = new[] { "method", "endpoint", "status_code" }
        });
    private static readonly Histogram RequestDuration = Metrics.CreateHistogram("http_request_duration_seconds", "Histogram of HTTP request durations", 
        new HistogramConfiguration
        {
            LabelNames = new[] { "method", "endpoint", "status_code" }
        });

    public PrometheusMetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            var statusCode = context.Response.StatusCode.ToString();
            var method = context.Request.Method;
            var endpoint = context.Request.Path;
            
            RequestCounter.WithLabels(method, endpoint, statusCode).Inc();
                
            RequestDuration.WithLabels(method, endpoint, statusCode).Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }
}