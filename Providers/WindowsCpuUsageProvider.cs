using System.Diagnostics;

public class WindowsCpuUsageProvider : IMetricProvider
{
    public string MetricName => "Cpu";
    private readonly PerformanceCounter _cpuCounter;

    public WindowsCpuUsageProvider()
    {
        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _cpuCounter.NextValue(); // First call is always 0
    }

    public Task<float> GetValueAsync()
    {
        return Task.FromResult(_cpuCounter.NextValue());
    }
}