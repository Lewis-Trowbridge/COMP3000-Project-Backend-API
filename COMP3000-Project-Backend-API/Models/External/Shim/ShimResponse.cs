using System.Text.Json.Serialization;

namespace COMP3000_Project_Backend_API.Models.External.Shim
{
    public class ShimResponse
    {
        [JsonPropertyName("temp")]
        public float Temperature { get; set; }
        public float? NO { get; set; } = 0;
        public float? NO2 { get; set; } = 0;
        public float? NOXasNO2 { get; set; } = 0;
        public float? O3 { get; set; } = 0;
        [JsonPropertyName("PM2.5")]
        public float? PM25 { get; set; } = 0;
        public float? PM10 { get; set; } = 0;
        public float? wd { get; set; } = 0;
        public float? ws { get; set; } = 0;
        public DateTime Timestamp { get; set; }
    }
}
