namespace COMP3000_Project_Backend_API.Models;

public class AirQualityInfo
{
    public float Value { get; set; }
    public string? Unit { get; set; }
    public DateTime Timestamp { get; set; }
    public string? LicenseInfo { get; set; }
    public Station? Station { get; set; }
}
