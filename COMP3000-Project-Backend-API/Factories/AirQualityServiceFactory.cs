using COMP3000_Project_Backend_API.Services;
using SimpleDateTimeProvider;

namespace COMP3000_Project_Backend_API.Factories
{
    public class AirQualityServiceFactory : IAirQualityServiceFactory
    {
        IServiceProvider _serviceProvider;
        IDateTimeProvider _dateTimeProvider;

        public AirQualityServiceFactory(IServiceProvider serviceProvider, IDateTimeProvider dateTimeProvider)
        {
            _serviceProvider = serviceProvider;
            _dateTimeProvider = dateTimeProvider;
        }
        public IAirQualityService GetAirQualityService(DateTime? requestTime)
        {
            var isFuture = requestTime > _dateTimeProvider.UtcNow;
            if (isFuture)
            {
                return _serviceProvider.GetRequiredService<PredictionsAirQualityService>();
            }
            else
            {
                return _serviceProvider.GetRequiredService<DEFRACsvService>();
            }
        }
    }
}
