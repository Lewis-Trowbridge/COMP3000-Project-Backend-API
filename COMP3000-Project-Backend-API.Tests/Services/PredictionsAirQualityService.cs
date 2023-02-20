using System;
using System.Net.Http.Json;
using System.Text.Json;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.External.Predictions;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class PredictionsAirQualityServiceTest
    {

        private static readonly double ValidOutput = 1d;
        private static readonly PredictionResponse ValidResponse = new PredictionResponse() { Outputs = new List<List<double>> { new List<double> { ValidOutput } } };

        [Fact]
        public async void PredictionsAirQualityService_Get_MakesCorrectHttpRequest()
        {
            var testStationId = "test";
            var testDateTime = DateTimeOffset.Parse("01-01-2022 04:00:00");
            var testAddress = PredictionsAirQualityService.BaseAddress + "/v1/models/airquality:predict";
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationId,
                Coords = new double[] { 0, 0 }
            };
            var expectedRequest = new PredictionRequest()
            {
                Inputs = new List<List<double>>{ new List<double>
            { testDateTime.ToUnixTimeSeconds(), testMetadata.Coords[1], testMetadata.Coords[0] }
            }
            };
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Post, testAddress)
                .ReturnsResponse(System.Net.HttpStatusCode.OK, JsonSerializer.Serialize(ValidResponse));

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(PredictionsAirQualityService.BaseAddress);

            var service = new PredictionsAirQualityService(client);

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime.UtcDateTime);

            handler.VerifyRequest(HttpMethod.Post, testAddress, async request =>
            {
                var content = await request.Content.ReadFromJsonAsync<PredictionRequest>();
                // Use serialise to get around value equality issues
                return JsonSerializer.Serialize(content) == JsonSerializer.Serialize(expectedRequest);
            }, Times.Once());
        }

        [Fact]
        public async void PredictionsAirQualityService_Get_ReturnsNullWithNullTimestamp()
        {
            var testStationId = "test";
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationId,
                Coords = new double[] { 0, 0 }
            };
            var service = new PredictionsAirQualityService(Mock.Of<HttpClient>());

            var actual = await service.GetAirQualityInfo(testMetadata, null);

            actual.Should().BeNull();
        }

        [Fact]
        public async void PredictionsAirQualityService_Get_ReturnsDataInAirQualityInfo()
        {
            var testStationId = "test";
            var testDateTime = DateTimeOffset.Parse("01-01-2022 04:00:00");
            var testAddress = PredictionsAirQualityService.BaseAddress + "/v1/models/airquality:predict";
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationId,
                Coords = new double[] { 0, 0 }
            };
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Post, testAddress)
                .ReturnsResponse(System.Net.HttpStatusCode.OK, JsonSerializer.Serialize(ValidResponse));

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(PredictionsAirQualityService.BaseAddress);

            var expected = new ReadingInfo()
            {
                Type = InfoType.Predicted,
                Timestamp = testDateTime.UtcDateTime,
                Value = Convert.ToSingle(ValidOutput),
                Unit = PredictionsAirQualityService.Unit,
                LicenseInfo = PredictionsAirQualityService.LicenseInfo,
                Station = testMetadata.ToStation(),
            };

            var service = new PredictionsAirQualityService(client);

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime.UtcDateTime);

            actual.Should().BeEquivalentTo(expected);

        }
    }
}
