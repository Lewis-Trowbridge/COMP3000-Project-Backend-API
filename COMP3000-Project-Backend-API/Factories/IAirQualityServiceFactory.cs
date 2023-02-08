using COMP3000_Project_Backend_API.Services;

namespace COMP3000_Project_Backend_API.Factories
{
    public interface IAirQualityServiceFactory
    {
        public IAirQualityService GetAirQualityService(DateTime? requestTime);
    }
}
