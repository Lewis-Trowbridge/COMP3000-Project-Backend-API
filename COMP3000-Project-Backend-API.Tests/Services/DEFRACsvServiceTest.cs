using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;
using Moq.Contrib.HttpClient;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class DEFRACsvServiceTest
    {
        [Fact]
        public async void DEFRACsvService_Get_MakesCorrectHttpRequest()
        {
            var testStationId = "test";
            var testDateTime = DateTime.MinValue;
            var testAddress = Constants.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, "");

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(Constants.DEFRABaseAddress);
            var service = new DEFRACsvService(client);

            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId
            };

            await service.GetAirQualityInfo(testMetadata, testDateTime);

            handler.VerifyRequest(testAddress, Times.Once());
        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsEmptyObjectOn404()
        {
            var testStationId = "test";
            var testDateTime = DateTime.MinValue;
            var testAddress = Constants.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.NotFound);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(Constants.DEFRABaseAddress);
            var service = new DEFRACsvService(client);

            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId
            };

            var expected = new AirQualityInfo();

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
