using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;

namespace COMP3000_Project_Backend_API.Services
{
    public interface ITemperatureService
    {
        public Task<ReadingInfo?> GetTemperatureInfo(DEFRAMetadata metadata, DateTime? timestamp);
    }
}
