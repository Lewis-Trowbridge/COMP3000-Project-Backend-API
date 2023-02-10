using System.Globalization;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using COMP3000_Project_Backend_API.IntegrationTests.Support;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models.Request;
using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.TestUtilities.Support;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SimpleDateTimeProvider;

namespace COMP3000_Project_Backend_API.FunctionalTests.Controllers
{
    public class AirQualityControllerTest : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<MongoDBFixture>, IDisposable
    {
        private HttpClient _client;
        private MongoDBFixture _mongoDBFixture;

        public AirQualityControllerTest(WebApplicationFactory<Program> webApplicationFactory, MongoDBFixture mongoDBFixture)
        {
            _mongoDBFixture = mongoDBFixture;
            mongoDBFixture.runner.Import("metadata", "metadata", "Assets/mongo.json", "--jsonArray");
            _client = webApplicationFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<MongoDBSettings>(options =>
                    {
                        options.ConnectionString = mongoDBFixture.runner.ConnectionString;
                        options.CollectionName = "metadata";
                        options.DatabaseName = "metadata";
                    });

                    services.AddSingleton(DEFRAUCsvUtilities.GetMockDateTimeProvider());
                    services.AddHttpClient<DEFRACsvService>()
                    .ConfigureHttpMessageHandlerBuilder(builder => builder.PrimaryHandler = DEFRAUCsvUtilities.GetHttpMessageHandler());
                });
            })
                .CreateClient();
        }

        [Fact]
        public async void AirQualityController_GetAirQuality_WithTimestampReturns200AndValidData()
        {
            var timestamp = DateTime.Parse("2022-01-04T01:00:00.000Z", CultureInfo.GetCultureInfo("en-GB"));
            var bbox = new BoundingBox()
            {
                BottomLeftX = 49.70890434294886,
                BottomLeftY = -13.168850856210385,
                TopRightX = 59.6111417069173,
                TopRightY = 1.9483351626784629
            };

            var response = await _client.GetAsync(ObjectToQueryString(bbox, timestamp));
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var stringActual = await response.Content.ReadAsStringAsync();
            // For parsing the unicode copyright symbol in the response
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
            var actual = JsonSerializer.Deserialize<AirQualityInfo[]>(stringActual, options);
            var expected = JsonSerializer.Deserialize<AirQualityInfo[]>(ValidJSON, options);

            actual.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public async void AirQualityController_GetAirQuality_WithoutTimestampReturns200AndValidData()
        {
            var bbox = new BoundingBox()
            {
                BottomLeftX = 49.70890434294886,
                BottomLeftY = -13.168850856210385,
                TopRightX = 59.6111417069173,
                TopRightY = 1.9483351626784629
            };

            var response = await _client.GetAsync(ObjectToQueryString(bbox));
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var stringActual = await response.Content.ReadAsStringAsync();
            // For parsing the unicode copyright symbol in the response
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
            var actual = JsonSerializer.Deserialize<AirQualityInfo[]>(stringActual, options);
            var expected = JsonSerializer.Deserialize<AirQualityInfo[]>(ValidNullTimestampJSON, options);

            actual.Should().BeEquivalentTo(expected);

        }

        [Theory]
        [InlineData(@"api/airquality?bbox.bottomLeftX=&bbox.bottomLeftY=1&bbox.topRightX=1&bbox.topRightY=1&timestamp=2022-01-04T01:00:00.000Z")]
        [InlineData(@"api/airquality?bbox.bottomLeftX=1&bbox.bottomLeftY=&bbox.topRightX=1&bbox.topRightY=1&timestamp=2022-01-04T01:00:00.000Z")]
        [InlineData(@"api/airquality?bbox.bottomLeftX=1&bbox.bottomLeftY=1&bbox.topRightX=&bbox.topRightY=1&timestamp=2022-01-04T01:00:00.000Z")]
        [InlineData(@"api/airquality?bbox.bottomLeftX=1&bbox.bottomLeftY=1&bbox.topRightX=1&bbox.topRightY=&timestamp=2022-01-04T01:00:00.000Z")]
        [InlineData(@"api/airquality?bbox.bottomLeftX=&bbox.bottomLeftY=&bbox.topRightX=&bbox.topRightY=&timestamp=2022-01-04T01:00:00.000Z")]

        public async void AirQualityController_GetAirQuality_MissingParamReturns400(string requestUri)
        {
            var response = await _client.GetAsync(requestUri);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        private static string ValidJSON { get; } = @"[{""value"":2.263,""unit"":""PM2.5"",""timestamp"":""2022-01-04T01:00:00+00:00"",""licenseInfo"":""\u00A9 Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence."",""station"":{""name"":""Aberdeen Erroll Park"",""coordinates"":{""lat"":57.1574,""lng"":-2.09477}}},{""value"":1.263,""unit"":""PM2.5"",""timestamp"":""2022-01-04T01:00:00+00:00"",""licenseInfo"":""\u00A9 Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence."",""station"":{""name"":""Aberdeen"",""coordinates"":{""lat"":57.15736,""lng"":-2.094278}}}]";
        private static string ValidNullTimestampJSON { get; } = @"[{""value"":2.792,""unit"":""PM2.5"",""timestamp"":""2022-01-11T00:00:00"",""licenseInfo"":""\u00A9 Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence."",""station"":{""name"":""Aberdeen Erroll Park"",""coordinates"":{""lat"":57.1574,""lng"":-2.09477}}},{""value"":1.792,""unit"":""PM2.5"",""timestamp"":""2022-01-11T00:00:00"",""licenseInfo"":""\u00A9 Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence."",""station"":{""name"":""Aberdeen"",""coordinates"":{""lat"":57.15736,""lng"":-2.094278}}}]";

        private static string ObjectToQueryString(BoundingBox bbox)
        {
            return $"api/airquality?bbox.bottomLeftX={bbox.BottomLeftX}&bbox.bottomLeftY={bbox.BottomLeftY}&bbox.topRightX={bbox.TopRightX}&bbox.topRightY={bbox.TopRightY}";

        }

        private static string ObjectToQueryString(BoundingBox bbox, DateTime timestamp)
        {
            return $"api/airquality?bbox.bottomLeftX={bbox.BottomLeftX}&bbox.bottomLeftY={bbox.BottomLeftY}&bbox.topRightX={bbox.TopRightX}&bbox.topRightY={bbox.TopRightY}&timestamp={timestamp.ToUniversalTime().ToString("u").Replace(" ", "T")}";
        }

        public void Dispose()
        {
            _mongoDBFixture.mongoClient.DropDatabase("metadata");
        }
    }
}
