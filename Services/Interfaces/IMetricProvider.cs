using Microsoft.AspNetCore.SignalR;
using YourApp.Hubs;
public class MetricReporterService : BackgroundService
{
    private readonly IHubContext<MetricHub> _hubContext; // Consider a more generic Hub
    private readonly IEnumerable<IMetricProvider> _metricProviders;
    private readonly ILogger<MetricReporterService> _logger;

    public MetricReporterService(IHubContext<MetricHub> hubContext,
                                 IEnumerable<IMetricProvider> metricProviders,
                                 ILogger<MetricReporterService> logger)
    {
        _hubContext = hubContext;
        _metricProviders = metricProviders;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);

            foreach (var provider in _metricProviders)
            {
                try
                {
                    var value = await provider.GetValueAsync();
                    await _hubContext.Clients.All.SendAsync($"Receive{provider.MetricName}Usage", value, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting metric for {MetricName}", provider.MetricName);
                }
            }
        }
    }
}