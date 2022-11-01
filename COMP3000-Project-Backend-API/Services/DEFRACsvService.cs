using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using CsvHelper;
using System.Globalization;

namespace COMP3000_Project_Backend_API.Services
{
    public class DEFRACsvService
    {
        private readonly HttpClient _httpClient;

        public DEFRACsvService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AirQualityInfo> GetAirQualityInfo(DEFRAMetadata stationInfo, DateTime timestamp)
        {
            var request = await _httpClient.GetAsync($"{stationInfo.Id}_PM25_{timestamp.Year}.csv");

            if (!request.IsSuccessStatusCode)
            {
                return new AirQualityInfo();
            }
            
            var contentString = await request.Content.ReadAsStringAsync();
            using (var contentStringReader = new StringReader(contentString))
            using (var csv = new CsvReader(contentStringReader, CultureInfo.InvariantCulture))
            {
                
            }

            return new AirQualityInfo();

        }
    }
}
