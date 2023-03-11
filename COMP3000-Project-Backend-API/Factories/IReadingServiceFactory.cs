using COMP3000_Project_Backend_API.Services;

namespace COMP3000_Project_Backend_API.Factories
{
    public interface IReadingServiceFactory
    {
        public IAirQualityService GetAirQualityService(DateTime? requestTime);
        public ITemperatureService GetTemperatureService(DateTime? requestTime);
    }
}
