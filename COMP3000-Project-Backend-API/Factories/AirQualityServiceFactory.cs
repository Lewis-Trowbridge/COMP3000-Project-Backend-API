using SimpleDateTimeProvider;
using COMP3000_Project_Backend_API.Services;

namespace COMP3000_Project_Backend_API.Factories
{
    public class AirQualityServiceFactory
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
                // TODO: Change to return predictions service
                return _serviceProvider.GetRequiredService<DEFRACsvService>();
            }
            else
            {
                return _serviceProvider.GetRequiredService<DEFRACsvService>();
            }
        }
    }
}
