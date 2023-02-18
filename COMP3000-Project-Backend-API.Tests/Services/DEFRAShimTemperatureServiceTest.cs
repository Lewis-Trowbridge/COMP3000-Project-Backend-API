using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.Utils;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class DEFRAShimTemperatureServiceTest
    {
        private static string TestStationId { get; } = "test";
        private static DateTime TestDateTime { get; } = DateTime.Parse("01-01-2022 04:00:00");
        private static DEFRAMetadata TestMetadata { get; } = new DEFRAMetadata()
        {
            Id = TestStationId,
            SiteName = TestStationId,
            Coords = new double[] { 0, 0 }
        };


        [Fact]
        public async void DEFRAShimTemperatureService_Get_MakesCorrectHttpRequest()
        {
            var testAddress = DEFRAShimTemperatureService.BaseAddress + $"/data?site=test&date=2022-01-01T04%3A00%3A00Z";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidResponseJSON);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRAShimTemperatureService.BaseAddress);
            var service = new DEFRAShimTemperatureService(client);

            await service.GetTemperatureInfo(TestMetadata, TestDateTime);

            handler.VerifyRequest(testAddress, Times.Once());
        }

        [Fact]
        public async void DEFRAShimTemperatureService_Get_ReturnsDataFromResponseWithTimestamp()
        {
            var testAddress = DEFRAShimTemperatureService.BaseAddress + $"/data?site=test&date=2022-01-01T04%3A00%3A00Z";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidResponseJSON);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRAShimTemperatureService.BaseAddress);
            var service = new DEFRAShimTemperatureService(client);

            var expected = new ReadingInfo()
            {
                LicenseInfo = DEFRAShimTemperatureService.LicenseString,
                Unit = DEFRAShimTemperatureService.Unit,
                Timestamp = TestDateTime,
                Station = TestMetadata.ToStation(),
                Value = ValidTemperatureReading
            };

            var actual = await service.GetTemperatureInfo(TestMetadata, TestDateTime);

            actual.Should().BeEquivalentTo(expected);

        }

        private static float ValidTemperatureReading = 1.0f;
        private static string ValidResponseJSON = @"{""temp"":1.0}";
    }
}
