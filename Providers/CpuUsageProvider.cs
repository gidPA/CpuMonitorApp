public abstract class CpuUsageProvider : IMetricProvider
{
    public string MetricName => "Cpu";
    private (ulong prevIdle, ulong prevTotal) _previousStats;

    public async Task<float> GetValueAsync()
    {
        var currentStats = await ReadCpuStatsAsync();
        var deltaIdle = currentStats.idle - _previousStats.prevIdle;
        var deltaTotal = currentStats.total - _previousStats.prevTotal;

        var cpuUsage = 100 * (1f - (float)deltaIdle / deltaTotal);

        _previousStats = currentStats;
        return cpuUsage;
    }

    protected abstract Task<(ulong idle, ulong total)> ReadCpuStatsAsync();
}