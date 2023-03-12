using System.Net.Http.Json;
using System.Text.Json;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.External.Predictions;
using COMP3000_Project_Backend_API.Models.External.Shim;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class PredictionsServiceTest
    {

        private static readonly double ValidOutput = 1d;
        private static readonly PredictionResponse ValidResponse = new PredictionResponse() { Outputs = new List<List<double>> { new List<double> { ValidOutput } } };

        [Fact]
        public async void PredictionsService_GetAirQuality_MakesCorrectHttpRequest()
        {
            var testStationId = "test";
            var testDateTime = DateTimeOffset.Parse("01-01-2022 04:00:00");
            var testAddress = PredictionsService.BaseAddress + "/v1/models/airquality:predict";
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
            client.BaseAddress = new Uri(PredictionsService.BaseAddress);

            var service = new PredictionsService(client, Mock.Of<IDEFRAShimService>());

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime.UtcDateTime);

            handler.VerifyRequest(HttpMethod.Post, testAddress, async request =>
            {
                var content = await request.Content.ReadFromJsonAsync<PredictionRequest>();
                // Use serialise to get around value equality issues
                return JsonSerializer.Serialize(content) == JsonSerializer.Serialize(expectedRequest);
            }, Times.Once());
        }

        [Fact]
        public async void PredictionsService_GetAirQuality_ReturnsNullWithNullTimestamp()
        {
            var testStationId = "test";
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationId,
                Coords = new double[] { 0, 0 }
            };
            var service = new PredictionsService(Mock.Of<HttpClient>(), Mock.Of<IDEFRAShimService>());

            var actual = await service.GetAirQualityInfo(testMetadata, null);

            actual.Should().BeNull();
        }

        [Fact]
        public async void PredictionsService_GetAirQuality_ReturnsDataInReadingInfo()
        {
            var testStationId = "test";
            var testDateTime = DateTimeOffset.Parse("01-01-2022 04:00:00");
            var testAddress = PredictionsService.BaseAddress + "/v1/models/airquality:predict";
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
            client.BaseAddress = new Uri(PredictionsService.BaseAddress);

            var expected = new ReadingInfo()
            {
                Type = InfoType.Predicted,
                Timestamp = testDateTime.UtcDateTime,
                Value = Convert.ToSingle(ValidOutput),
                Unit = DEFRACsvService.PM25Unit,
                LicenseInfo = PredictionsService.LicenseInfo,
                Station = testMetadata.ToStation(),
            };

            var service = new PredictionsService(client, Mock.Of<IDEFRAShimService>());

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime.UtcDateTime);

            actual.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public async void PredictionsService_GetTemperature_MakesCorrectHttpRequest()
        {
            var testStationId = "test";
            var testDateTime = DateTime.Parse("01-01-2022 04:00:00");
            var expectedTimestamp = 1641009600;
            var testAddress = PredictionsService.BaseAddress + "/v1/models/temperature:predict";
            
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationId,
                Coords = new double[] { 0, 0 }
            };

            var fakeShimResponse = new ShimResponse()
            {
                NO = 1,
                NO2 = 2,
                NOXasNO2 = 3,
                O3 = 4,
                PM10 = 5,
                PM25 = 6,
                wd = 7,
                ws = 8,
            };
            var mockShimservice = new Mock<IDEFRAShimService>();
            mockShimservice.Setup(x => x.GetDataFromShim(testMetadata, null))
                .ReturnsAsync(fakeShimResponse);
            var expectedRequest = new PredictionRequest()
            {
                Inputs = new List<List<double>>{ new List<double>{
                    expectedTimestamp,
                    fakeShimResponse.NO.Value,
                    fakeShimResponse.NO2.Value,
                    fakeShimResponse.NOXasNO2.Value,
                    fakeShimResponse.O3.Value,
                    fakeShimResponse.PM10.Value,
                    fakeShimResponse.PM25.Value,
                    fakeShimResponse.wd.Value,
                    fakeShimResponse.ws.Value,
                    testMetadata.Coords[1],
                    testMetadata.Coords[0] 
                }}
            };
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Post, testAddress)
                .ReturnsResponse(System.Net.HttpStatusCode.OK, JsonSerializer.Serialize(ValidResponse));

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(PredictionsService.BaseAddress);

            var service = new PredictionsService(client, mockShimservice.Object);

            var actual = await service.GetTemperatureInfo(testMetadata, testDateTime);

            handler.VerifyRequest(HttpMethod.Post, testAddress, async request =>
            {
                var content = await request.Content.ReadFromJsonAsync<PredictionRequest>();
                // Use serialise to get around value equality issues
                return JsonSerializer.Serialize(content) == JsonSerializer.Serialize(expectedRequest);
            }, Times.Once());
        }

        [Fact]
        public async void PredictionsService_GetTemperature_ReturnsDateInReadingInfo()
        {
            var testStationId = "test";
            var testDateTime = DateTime.Parse("01-01-2022 04:00:00");
            var expectedTimestamp = 1641009600;
            var testAddress = PredictionsService.BaseAddress + "/v1/models/temperature:predict";

            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationId,
                Coords = new double[] { 0, 0 }
            };

            var fakeShimResponse = new ShimResponse()
            {
                NO = 1,
                NO2 = 2,
                NOXasNO2 = 3,
                O3 = 4,
                PM10 = 5,
                PM25 = 6,
                wd = 7,
                ws = 8,
            };
            var mockShimservice = new Mock<IDEFRAShimService>();
            mockShimservice.Setup(x => x.GetDataFromShim(testMetadata, null))
                .ReturnsAsync(fakeShimResponse);
            
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Post, testAddress)
                .ReturnsResponse(System.Net.HttpStatusCode.OK, JsonSerializer.Serialize(ValidResponse));

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(PredictionsService.BaseAddress);

            var expected = new ReadingInfo()
            {
                Type = InfoType.Predicted,
                Timestamp = testDateTime,
                Value = Convert.ToSingle(ValidOutput),
                Unit = DEFRAShimTemperatureService.Unit,
                LicenseInfo = PredictionsService.LicenseInfo,
                Station = testMetadata.ToStation(),
            };

            var service = new PredictionsService(client, mockShimservice.Object);

            var actual = await service.GetTemperatureInfo(testMetadata, testDateTime);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
