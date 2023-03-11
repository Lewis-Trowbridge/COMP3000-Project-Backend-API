using System.Text.Json.Serialization;

namespace COMP3000_Project_Backend_API.Models.External.Shim
{
    public class ShimResponse
    {
        [JsonPropertyName("temp")]
        public float? Temperature { get; set; }
        public double? NO { get; set; }
        public double? NO2 { get; set; }
        public double? NOXasNO2 { get; set; }
        public double? O3 { get; set; }
        [JsonPropertyName("PM2.5")]
        public double? PM25 { get; set; }
        public double? PM10 { get; set; }
        public double? wd { get; set; }
        public double? ws { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
