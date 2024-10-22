using System.Text.RegularExpressions;
using Prometheus;

namespace FcadHackProxy.Middlewares;

public class MetricsMiddleware
{
    private readonly RequestDelegate Next;

    /// <summary>
    /// Contructor taking all dependencies
    /// </summary>
    /// <param name="next">Next delegate in the chain, where the request will be passed to</param>
    public MetricsMiddleware(RequestDelegate next)
    {
        this.Next = next;
    }

    /// <summary>
    /// Method called when this middleware is invoked
    /// </summary>
    /// <param name="context">HTTP request/response context</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var pathRegex = new Regex("api/(.*?)(/|\\?|$)");
        var matches = pathRegex.Match(context.Request.Path.ToString());
        // Skip adding any metrics if the endpoint isn't a /api/ endpoint
        if (matches.Length <= 0 || matches.Groups.Count <= 0)
        {
            await this.Next(context);
            return;
        }

        var path = matches.Groups[1].ToString();
        var histogram = Metrics.CreateHistogram($"{path}_{method}_duration", $"Histogram timer of {method} calls to {path}");
        var gauge = Metrics.CreateGauge($"{path}_{method}_gague", $"Number of {method} requests to {path} currently in progress");
        // Time all requests and increment/decrement the gague
        using (histogram.NewTimer())
        {
            gauge.Inc();
            await this.Next(context);
            gauge.Dec();
        }
    }

}