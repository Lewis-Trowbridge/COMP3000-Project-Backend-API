using System.Text.Json;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models.Predictions;

namespace COMP3000_Project_Backend_API.Services
{
    public class PredictionsAirQualityService : IAirQualityService
    {
        public static string BaseAddress { get; } = "https://predictions-xsji6nno4q-ew.a.run.app";
        public static string Unit = "PM2.5";
        public static string LicenseInfo = "\u00A9 Lewis Trowbridge 2023";

        private readonly HttpClient _httpClient;

        public PredictionsAirQualityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AirQualityInfo?> GetAirQualityInfo(DEFRAMetadata metadata, DateTime? timestamp)
        {
            if (!timestamp.HasValue)
            {
                return null;
            }
            else
            {
                var utcTimestamp = DateTime.SpecifyKind(timestamp.Value, DateTimeKind.Utc);
                var request = new PredictionRequest()
                {
                    Inputs = new List<List<double>> { new List<double>{
                            Convert.ToDouble(((DateTimeOffset)utcTimestamp).ToUnixTimeSeconds()),
                            // Reverse long/lat metadata to lat/long 
                            metadata.Coords[1],
                            metadata.Coords[0]
                        } }
                };
                var response = await _httpClient.PostAsJsonAsync("v1/models/airquality:predict", request);
                var data = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                if (data != null)
                {
                    return new AirQualityInfo()
                    {
                        Timestamp = utcTimestamp,
                        Value = Convert.ToSingle(data.Outputs[0][0]),
                        Unit = Unit,
                        Station = metadata.ToStation(),
                        LicenseInfo = LicenseInfo,
                    };
                }
                return null;
            }
        }
    }
}
