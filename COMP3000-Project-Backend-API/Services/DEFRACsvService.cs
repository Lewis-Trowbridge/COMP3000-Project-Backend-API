using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;

namespace COMP3000_Project_Backend_API.Services
{
    public class DEFRACsvService
    {
        private readonly HttpClient _httpClient;

        public DEFRACsvService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AirQualityInfo> GetAirQualityInfo(DEFRAMetadata stationInfo)
        {
            var request = await _httpClient.GetAsync(stationInfo.Id);

            return new AirQualityInfo();

        }
    }
}
