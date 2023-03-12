using System.Globalization;
using COMP3000_Project_Backend_API.Factories;
using COMP3000_Project_Backend_API.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleDateTimeProvider;

namespace COMP3000_Project_Backend_API.Tests.Factories
{
    public class ReadingServiceFactoryTest
    {
        [Fact]
        public void ReadingServiceFactory_WithPastTimestamp_ReturnsDEFRACsvService()
        {
            var currentTime = DateTime.Parse("02-01-2022 00:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var mockDateTimeProvider = new MockDateTimeProvider
            {
                UtcNow = currentTime
            };
            var serviceCollection = new ServiceCollection();
            var service = new DEFRACsvService(Mock.Of<HttpClient>(), mockDateTimeProvider);
            serviceCollection.AddSingleton(service);

            var factory = new ReadingServiceFactory(serviceCollection.BuildServiceProvider(), mockDateTimeProvider);

            var actual = factory.GetAirQualityService(currentTime.AddDays(-1));

            actual.Should().BeOfType<DEFRACsvService>();
        }

        [Fact]
        public void ReadingServiceFactory_AirQualityWithCurrentTimestamp_ReturnsDEFRACsvService()
        {
            var currentTime = DateTime.Parse("02-01-2022 00:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var mockDateTimeProvider = new MockDateTimeProvider
            {
                UtcNow = currentTime
            };
            var serviceCollection = new ServiceCollection();
            var service = new DEFRACsvService(Mock.Of<HttpClient>(), mockDateTimeProvider);
            serviceCollection.AddSingleton(service);

            var factory = new ReadingServiceFactory(serviceCollection.BuildServiceProvider(), mockDateTimeProvider);

            var actual = factory.GetAirQualityService(currentTime);

            actual.Should().BeOfType<DEFRACsvService>();
        }

        [Fact]
        public void ReadingServiceFactory_AirQualityWithFutureTimestamp_ReturnsPredictionsService()
        {
            var currentTime = DateTime.Parse("02-01-2022 00:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var mockDateTimeProvider = new MockDateTimeProvider
            {
                UtcNow = currentTime
            };
            var serviceCollection = new ServiceCollection();
            var service = new PredictionsService(Mock.Of<HttpClient>(), Mock.Of<IDEFRAShimService>());
            serviceCollection.AddSingleton(service);

            var factory = new ReadingServiceFactory(serviceCollection.BuildServiceProvider(), mockDateTimeProvider);

            var actual = factory.GetAirQualityService(currentTime.AddDays(1));

            actual.Should().BeOfType<PredictionsService>();
        }

        [Fact]
        public void ReadingServiceFactory_TemperatureWithCurrentTimestamp_ReturnsDEFRAShimTemperatureService()
        {
            var currentTime = DateTime.Parse("02-01-2022 00:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var mockDateTimeProvider = new MockDateTimeProvider
            {
                UtcNow = currentTime
            };
            var serviceCollection = new ServiceCollection();
            var service = new DEFRAShimTemperatureService(Mock.Of<IDEFRAShimService>());
            serviceCollection.AddSingleton(service);

            var factory = new ReadingServiceFactory(serviceCollection.BuildServiceProvider(), mockDateTimeProvider);

            var actual = factory.GetTemperatureService(currentTime);

            actual.Should().BeOfType<DEFRAShimTemperatureService>();
        }

        [Fact]
        public void ReadingServiceFactory_TemperatureWithFutureTimestamp_ReturnsPredictionsService()
        {
            var currentTime = DateTime.Parse("02-01-2022 00:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var mockDateTimeProvider = new MockDateTimeProvider
            {
                UtcNow = currentTime
            };
            var serviceCollection = new ServiceCollection();
            var service = new PredictionsService(Mock.Of<HttpClient>(), Mock.Of<IDEFRAShimService>());
            serviceCollection.AddSingleton(service);

            var factory = new ReadingServiceFactory(serviceCollection.BuildServiceProvider(), mockDateTimeProvider);

            var actual = factory.GetTemperatureService(currentTime.AddDays(1));

            actual.Should().BeOfType<PredictionsService>();
        }
    }
}
