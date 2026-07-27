// src/DevBoard.Infrastructure/BackgroundServices/WebhookDeliveryWorker.cs
using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevBoard.Infrastructure.BackgroundServices;

public sealed class WebhookDeliveryWorker : BackgroundService
{
    private readonly WebhookChannel _channel;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<WebhookDeliveryWorker> _logger;

    public WebhookDeliveryWorker(WebhookChannel channel, IHttpClientFactory httpFactory, ILogger<WebhookDeliveryWorker> logger)
    {
        _channel = channel;
        _httpFactory = httpFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WebhookDeliveryWorker starting");

        await foreach (var payload in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var client = _httpFactory.CreateClient("webhook");
                var response = await client.PostAsJsonAsync(payload.TargetUrl, payload, stoppingToken);
                _logger.LogInformation("Webhook delivered: {StatusCode} to {Url}", response.StatusCode, payload.TargetUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook delivery failed for {Url}", payload.TargetUrl);
            }
        }
    }
}