using COMP3000_Project_Backend_API.Services;
using SimpleDateTimeProvider;

namespace COMP3000_Project_Backend_API.Factories
{
    public class ReadingServiceFactory : IReadingServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReadingServiceFactory(IServiceProvider serviceProvider, IDateTimeProvider dateTimeProvider)
        {
            _serviceProvider = serviceProvider;
            _dateTimeProvider = dateTimeProvider;
        }
        public IAirQualityService GetAirQualityService(DateTime? requestTime)
        {
            var isFuture = requestTime > _dateTimeProvider.UtcNow;
            if (isFuture)
            {
                return _serviceProvider.GetRequiredService<PredictionsService>();
            }
            else
            {
                return _serviceProvider.GetRequiredService<DEFRACsvService>();
            }
        }

        public ITemperatureService GetTemperatureService(DateTime? requestTime)
        {
            var isFuture = requestTime > _dateTimeProvider.UtcNow;
            if (isFuture)
            {
                return _serviceProvider.GetRequiredService<PredictionsService>();
            }
            else
            {
                return _serviceProvider.GetRequiredService<DEFRAShimTemperatureService>();
            }
        }
    }
}
