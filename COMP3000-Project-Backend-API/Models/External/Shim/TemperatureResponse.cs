using System.Text.Json.Serialization;

namespace COMP3000_Project_Backend_API.Models.External.Shim
{
    public class TemperatureResponse
    {
        [JsonPropertyName("temp")]
        public float Temperature { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
