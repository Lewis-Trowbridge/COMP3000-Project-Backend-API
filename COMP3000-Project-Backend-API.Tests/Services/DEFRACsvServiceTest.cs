using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;
using Moq.Contrib.HttpClient;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class DEFRACsvServiceTest
    {
        private readonly string baseAddress = "https://uk-air.defra.gov.uk/datastore/data_files/site_data/";

        [Fact]
        public async void DEFRACsvService_Get_MakesCorrectHttpRequest()
        {
            var testStationId = "test";
            var testAddress = baseAddress + testStationId;
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(baseAddress);
            var service = new DEFRACsvService(client);

            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId
            };

            await service.GetAirQualityInfo(testMetadata);

            handler.VerifyRequest(testAddress, Times.Once());
        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsEmptyObjectOn404()
        {
            var testStationId = "test";
            var testAddress = baseAddress + testStationId;
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.NotFound);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(baseAddress);
            var service = new DEFRACsvService(client);

            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId
            };

            var expected = new AirQualityInfo();

            var actual = await service.GetAirQualityInfo(testMetadata);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
