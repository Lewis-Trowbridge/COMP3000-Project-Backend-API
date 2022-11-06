using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models;

namespace COMP3000_Project_Backend_API.Services
{
    public interface IAirQualityService
    {
        public Task<AirQualityInfo?> GetAirQualityInfo(DEFRAMetadata metadata, DateTime? timestamp);
    }
}
