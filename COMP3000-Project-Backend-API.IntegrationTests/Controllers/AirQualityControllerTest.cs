using System.Globalization;
using COMP3000_Project_Backend_API.Controllers;
using COMP3000_Project_Backend_API.IntegrationTests.Support;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models.Request;
using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.Factories;
using COMP3000_Project_Backend_API.TestUtilities.Support;
using MongoDB.Driver;
using SimpleDateTimeProvider;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace COMP3000_Project_Backend_API.IntegrationTests.Controllers
{
    // Set up
    public class AirQualityControllerTest : IClassFixture<MongoDBFixture>, IDisposable
    {
        private MongoDBFixture _mongoDBFixture;
        private MongoClient _mongoClient;
        private IMongoCollection<DEFRAMetadata> _collection;

        public AirQualityControllerTest(MongoDBFixture mongoDBFixture)
        {
            _mongoDBFixture = mongoDBFixture;
            mongoDBFixture.runner.Import("metadata", "metadata", "Assets/mongo.json", "--jsonArray");
            _mongoClient = _mongoDBFixture.mongoClient;
            _collection = _mongoClient.GetDatabase("metadata").GetCollection<DEFRAMetadata>("metadata");
        }

        [Fact]
        public async void AirQualityController_GetAirQuality_GetsValidData()
        {
            var metadataService = new MetadataService(_collection);
            var airQualityService = new DEFRACsvService(DEFRAUCsvUtilities.GetMockHttpClient(), new SystemDateTimeProvider());
            var timestamp = DateTime.Parse("04-01-2022 01:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(airQualityService);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var airQualityFactory = new AirQualityServiceFactory(serviceProvider, new SystemDateTimeProvider());
            var controller = new AirQualityController(metadataService, airQualityFactory);

            var request = new AirQualityRequest()
            {
                Bbox = new BoundingBox(49.70890434294886, -13.168850856210385, 59.6111417069173, 1.9483351626784629),
                Timestamp = timestamp
            };

            var expected = new AirQualityInfo[]
            {
                new AirQualityInfo()
                {
                    Value = 1.263f,
                    Unit = DEFRACsvService.PM25Unit,
                    Timestamp = timestamp,
                    LicenseInfo = DEFRACsvService.LicenseString,
                    Station = new Station()
                    {
                        Name = "Aberdeen",
                        Coordinates = new LatLong()
                        {
                            Lat = 57.15736,
                            Lng = -2.094278
                        }
                    }
                },

                new AirQualityInfo()
                {
                    Value = 2.263f,
                    Unit = DEFRACsvService.PM25Unit,
                    Timestamp = timestamp,
                    LicenseInfo = DEFRACsvService.LicenseString,
                    Station = new Station()
                    {
                        Name = "Aberdeen Erroll Park",
                        Coordinates = new LatLong()
                        {
                            Lat = 57.1574,
                            Lng = -2.09477
                        }
                    }
                },
            };

            var actual = await controller.GetAirQuality(request);

            actual.Should().BeEquivalentTo(expected);
        }

        // Tear down
        public void Dispose()
        {
            _mongoClient.DropDatabase("metadata");
        }
    }
}
