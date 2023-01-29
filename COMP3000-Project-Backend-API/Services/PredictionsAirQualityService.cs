using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;

namespace COMP3000_Project_Backend_API.Services
{
    public class PredictionsAirQualityService : IAirQualityService
    {
        public static string BaseAddress { get; } = "https://predictions-xsji6nno4q-ew.a.run.app";

        public Task<AirQualityInfo?> GetAirQualityInfo(DEFRAMetadata metadata, DateTime? timestamp)
        {
            throw new NotImplementedException();
        }
    }
}
