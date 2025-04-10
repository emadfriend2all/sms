namespace Showmatics.Application.Dashboard;

public class StatsDto
{
    public int ItemCount { get; set; }
    public int BrandCount { get; set; }
    public int UserCount { get; set; }
    public int RoleCount { get; set; }
    public List<ChartSeries> DataEnterBarChart { get; set; } = new();
    public Dictionary<string, double>? ItemByBrandTypePieChart { get; set; }
}

public class ChartSeries
{
    public string? Name { get; set; }
    public double[]? Data { get; set; }
}