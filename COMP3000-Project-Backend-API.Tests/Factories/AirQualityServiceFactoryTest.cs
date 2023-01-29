using COMP3000_Project_Backend_API.Factories;
using COMP3000_Project_Backend_API.Services;
using System.Globalization;
using SimpleDateTimeProvider;
using Microsoft.Extensions.DependencyInjection;

namespace COMP3000_Project_Backend_API.Tests.Factories
{
    public class AirQualityServiceFactoryTest
    {
        [Fact]
        public void AirQualityServiceFactory_WithPastTimestamp_ReturnsDEFRACsvService()
        {
            var currentTime = DateTime.Parse("02-01-2022 00:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var mockDateTimeProvider = new MockDateTimeProvider
            {
                UtcNow = currentTime
            };
            var serviceCollection = new ServiceCollection();
            var service = new DEFRACsvService(Mock.Of<HttpClient>(), mockDateTimeProvider);
            serviceCollection.AddSingleton(service);

            var factory = new AirQualityServiceFactory(serviceCollection.BuildServiceProvider(), mockDateTimeProvider);

            var actual = factory.GetAirQualityService(currentTime.AddDays(-1));

            actual.Should().BeOfType<DEFRACsvService>();
        }

        [Fact]
        public void AirQualityServiceFactory_WithCurrentTimestamp_ReturnsDEFRACsvService()
        {
            var currentTime = DateTime.Parse("02-01-2022 00:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var mockDateTimeProvider = new MockDateTimeProvider
            {
                UtcNow = currentTime
            };
            var serviceCollection = new ServiceCollection();
            var service = new DEFRACsvService(Mock.Of<HttpClient>(), mockDateTimeProvider);
            serviceCollection.AddSingleton(service);

            var factory = new AirQualityServiceFactory(serviceCollection.BuildServiceProvider(), mockDateTimeProvider);

            var actual = factory.GetAirQualityService(currentTime);

            actual.Should().BeOfType<DEFRACsvService>();
        }
    }
}
