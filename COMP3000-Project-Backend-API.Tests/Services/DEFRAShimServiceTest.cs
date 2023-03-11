using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class DEFRAShimServiceTest
    {
        private static string TestStationId { get; } = "test";
        private static string ValidResponseJSON = @"{""temp"":1.0,""timestamp"":""2022-01-01T04:00:00Z""}";
        private static DateTime TestDateTime { get; } = DateTime.Parse("01-01-2022 04:00:00");
        private static DEFRAMetadata TestMetadata { get; } = new DEFRAMetadata()
        {
            Id = TestStationId,
            SiteName = TestStationId,
            Coords = new double[] { 0, 0 }
        };


        [Fact]
        public async void DEFRAShimService_Get_MakesCorrectHttpRequestWithTimestamp()
        {
            var testAddress = DEFRAShimService.BaseAddress + $"/data?site=test&date=2022-01-01T04%3A00%3A00Z";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidResponseJSON);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRAShimService.BaseAddress);
            var service = new DEFRAShimService(client);

            await service.GetDataFromShim(TestMetadata, TestDateTime);

            handler.VerifyRequest(testAddress, Times.Once());
        }

        [Fact]
        public async void DEFRAShimService_Get_MakesCorrectHttpRequestWithNoTimestamp()
        {
            var testAddress = DEFRAShimService.BaseAddress + $"/data?site=test";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidResponseJSON);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRAShimService.BaseAddress);
            var service = new DEFRAShimService(client);

            await service.GetDataFromShim(TestMetadata, null);

            handler.VerifyRequest(testAddress, Times.Once());
        }

        [Fact]
        public async void DEFRAShimTemperatureService_Get_ReturnsNullWhenRecieving404()
        {
            var testAddress = DEFRAShimService.BaseAddress + $"/data?site=test&date=2022-01-01T04%3A00%3A00Z";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.NotFound);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRAShimService.BaseAddress);
            var service = new DEFRAShimService(client);

            var actual = await service.GetDataFromShim(TestMetadata, TestDateTime);

            actual.Should().BeNull();

        }
    }
}
