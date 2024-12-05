using System;

namespace RoadStatus;

public class RoadStatusResponse
{
    public string? DisplayName { get; set; }
    public string? StatusSeverity { get; set; }
    public string? StatusSeverityDescription { get; set; }
    public string? ErroCode { get; set; }
    public int ExitCode { get; set; }
}
