public interface IMetricProvider
{
    string MetricName { get; }
    Task<float> GetValueAsync();
}