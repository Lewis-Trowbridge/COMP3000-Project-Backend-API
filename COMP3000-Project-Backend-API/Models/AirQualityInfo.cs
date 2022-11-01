namespace COMP3000_Project_Backend_API.Models;

public class AirQualityInfo
{
    public float value;
    public string? unit;
    public DateTime timestamp;
    public string? licenseInfo;
    public Station? station;
}
