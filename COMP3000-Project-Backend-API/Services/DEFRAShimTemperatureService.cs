using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.External.Shim;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Utils;

namespace COMP3000_Project_Backend_API.Services
{
    public class DEFRAShimTemperatureService : ITemperatureService
    {
        public static string Unit { get; } = "°C";
        public static string LicenseString { get; } = "\u00A9 Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence.";
        public static string BaseAddress { get; } = "https://defra-shim-xsji6nno4q-ew.a.run.app";

        private readonly HttpClient _httpClient;
        public DEFRAShimTemperatureService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ReadingInfo?> GetTemperatureInfo(DEFRAMetadata metadata, DateTime? timestamp)
        {
            var queryValues = new Dictionary<string, string?>
            {
                { "site", metadata.Id },
            };

            if (timestamp is not null) {
                queryValues.Add("date", timestamp.Value.ToIsoTimestamp());
            }

            var response = await _httpClient.GetAsync("/data" + QueryString.Create(queryValues).ToString());
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TemperatureResponse>();
                return new ReadingInfo()
                {
                    LicenseInfo = LicenseString,
                    Unit = Unit,
                    Timestamp = data.Timestamp,
                    Station = metadata.ToStation(),
                    Value = data.Temperature
                };
            }
            return null;
        }
    }
}
