using Microsoft.AspNetCore.SignalR;
using CpuMonitorApp.Hubs;
using Microsoft.AspNetCore.Mvc;

namespace CpuMonitorApp.Services;

public class CpuMonitorService : BackgroundService
{
    private readonly IHubContext<CpuHub> _hubContext;
    private readonly ILogger<CpuMonitorService> _logger;

    public CpuMonitorService(IHubContext<CpuHub> hubContext, ILogger<CpuMonitorService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        (ulong prevIdle, ulong prevTotal) = ReadCpuStats();

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
            (ulong idle, ulong total) = ReadCpuStats();

            ulong deltaIdle = idle - prevIdle;
            ulong deltaTotal = total - prevTotal;

            float cpuUsage = 100 * (1f - (float)deltaIdle / deltaTotal);

            prevIdle = idle;
            prevTotal = total;

            //_logger.LogInformation("CPU Usage: {CpuUsage}%", cpuUsage);

            await _hubContext.Clients.All.SendAsync("ReceiveCpuUsage", cpuUsage, cancellationToken: stoppingToken);
        }
    }

    private (ulong idle, ulong total) ReadCpuStats()
    {
        var line = File.ReadLines("/proc/stat").FirstOrDefault(l => l.StartsWith("cpu "));
        if (line == null)
        {
            return (0, 0);
        }

        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(ulong.Parse).ToArray();

        ulong user = parts[0];
        ulong nice = parts[1];
        ulong system = parts[2];
        ulong idle = parts[3];
        ulong iowait = parts[4];
        ulong irq = parts[5];
        ulong softirq = parts[6];
        ulong steal = parts.Length > 7 ? parts[7] : 0;

        ulong totalIdle = idle + iowait;

        ulong total = 0;
        foreach (var part in parts)
        {
            total += part;
        }

        return (totalIdle, total);
    }
}