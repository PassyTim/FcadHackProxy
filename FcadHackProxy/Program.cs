using FcadHackProxy.Data;
using FcadHackProxy.FilteringSettings;
using FcadHackProxy.Middlewares;
using FcadHackProxy.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson();
    
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<MessageFilterService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("cache:6379,abortConnect=false,connectTimeout=10000"));
builder.Services.AddSingleton<FilterSettings>();
builder.Services.AddSingleton<FilterSettingsService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<MongoDbService>();
builder.Services.AddScoped<ServerRepository>();

builder.Services.AddHttpClient<SendRequestService>();
builder.Services.AddMetrics();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName))
    .WithMetrics(metrics =>
            metrics
                .AddAspNetCoreInstrumentation() 
                .AddRuntimeInstrumentation() 
                .AddPrometheusExporter()
    );
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin();
        corsPolicyBuilder.AllowAnyHeader();
        corsPolicyBuilder.AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseMetricServer();
app.UseHttpMetrics();

app.UseMiddleware<PrometheusMetricsMiddleware>();
app.UseMiddleware<MetricsMiddleware>();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapControllers();
app.MapMetrics();

app.Run();