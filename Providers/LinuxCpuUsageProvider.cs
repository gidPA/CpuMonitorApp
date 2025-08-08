public class LinuxCpuUsageProvider : CpuUsageProvider
{
    protected override async Task<(ulong idle, ulong total)> ReadCpuStatsAsync()
    {
        string line = "";
        await foreach (var l in File.ReadLinesAsync("/proc/stat"))
        {
            if (l.StartsWith("cpu "))
            {
                line = l;
                break;
            }
        }
        if (line == null)
        {
            return (0, 0);
        }

        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(ulong.Parse).ToArray();
        var idle = parts[3] + parts[4]; // idle + iowait
        var total = parts.Aggregate((ulong)0, (acc, val) => acc + val);

        return (idle, total);
    }
}