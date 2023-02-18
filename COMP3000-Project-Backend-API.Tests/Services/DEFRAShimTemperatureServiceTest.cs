using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.Utils;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class DEFRAShimTemperatureServiceTest
    {
        [Fact]
        public async void DEFRAShimTemperatureServiceTest_Get_MakesCorrectHttpRequest()
        {
            var testStationId = "test";
            var testDateTime = DateTime.Parse("01-01-2022 04:00:00");
            var testAddress = DEFRAShimTemperatureService.BaseAddress + $"/data?site=test&date=2022-01-01T04%3A00%3A00Z";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidResponseJSON);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRAShimTemperatureService.BaseAddress);
            var service = new DEFRAShimTemperatureService(client);

            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationId,
                Coords = new double[] { 0, 0 }
            };

            await service.GetTemperatureInfo(testMetadata, testDateTime);

            handler.VerifyRequest(testAddress, Times.Once());
        }

        private static string ValidResponseJSON = @"{""temp"":1.0}";
    }
}
