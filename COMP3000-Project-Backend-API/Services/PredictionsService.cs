using System.Text.Json;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.External.Predictions;
using COMP3000_Project_Backend_API.Models.MongoDB;

namespace COMP3000_Project_Backend_API.Services
{
    public class PredictionsService : IAirQualityService, ITemperatureService
    {
        public static string BaseAddress { get; } = "https://predictions-xsji6nno4q-ew.a.run.app";
        public static string LicenseInfo = "\u00A9 Lewis Trowbridge 2023";

        private readonly HttpClient _httpClient;
        private readonly IDEFRAShimService _shimService;

        public PredictionsService(HttpClient httpClient, IDEFRAShimService shimService)
        {
            _httpClient = httpClient;
            _shimService = shimService;
        }

        public async Task<ReadingInfo?> GetAirQualityInfo(DEFRAMetadata metadata, DateTime? timestamp)
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
                            ((DateTimeOffset)utcTimestamp).ToUnixTimeSeconds(),
                            // Reverse long/lat metadata to lat/long 
                            metadata.Coords[1],
                            metadata.Coords[0]
                        } }
                };
                var response = await _httpClient.PostAsJsonAsync("v1/models/airquality:predict", request);
                var data = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                if (data != null)
                {
                    return new ReadingInfo()
                    {
                        Type = InfoType.Predicted,
                        Timestamp = utcTimestamp,
                        Value = Convert.ToSingle(data.Outputs[0][0]),
                        Unit = DEFRACsvService.PM25Unit,
                        Station = metadata.ToStation(),
                        LicenseInfo = LicenseInfo,
                    };
                }
                return null;
            }
        }

        public async Task<ReadingInfo?> GetTemperatureInfo(DEFRAMetadata metadata, DateTime? timestamp)
        {
            var utcTimestamp = DateTime.SpecifyKind(timestamp.Value, DateTimeKind.Utc);
            var latestData = await _shimService.GetDataFromShim(metadata, null);
            if (latestData is not null)
            {
                var request = new PredictionRequest()
                {
                    Inputs = new List<List<double>> { new List<double>{
                            ((DateTimeOffset)utcTimestamp).ToUnixTimeSeconds(),
                            latestData.NO ?? 0,
                            latestData.NO2 ?? 0,
                            latestData.NOXasNO2 ?? 0,
                            latestData.O3 ?? 0,
                            latestData.PM10 ?? 0,
                            latestData.PM25 ?? 0,
                            latestData.wd ?? 0,
                            latestData.ws ?? 0,
                            // Reverse long/lat metadata to lat/long 
                            metadata.Coords[1],
                            metadata.Coords[0]
                        } }
                };
                var response = await _httpClient.PostAsJsonAsync("v1/models/temperature:predict", request);
                var data = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                if (data != null)
                {
                    return new ReadingInfo()
                    {
                        Type = InfoType.Predicted,
                        Timestamp = utcTimestamp,
                        Value = Convert.ToSingle(data.Outputs[0][0]),
                        Unit = DEFRAShimTemperatureService.Unit,
                        Station = metadata.ToStation(),
                        LicenseInfo = LicenseInfo,
                    };
                }
            }
            return null;
        }
    }
}
