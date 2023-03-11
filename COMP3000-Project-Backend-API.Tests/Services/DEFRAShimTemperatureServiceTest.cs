using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.External.Shim;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;

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
        public async void DEFRAShimTemperatureService_Get_ReturnsDataFromServiceAsReadingInfo()
        {
            var fakeShimServiceResponse = new ShimResponse()
            {
                Temperature = ValidTemperatureReading,
                Timestamp = TestDateTime
            };

            var mockShimService = new Mock<IDEFRAShimService>();
            mockShimService.Setup(x => x.GetDataFromShim(TestMetadata, TestDateTime)).ReturnsAsync(fakeShimServiceResponse);

            var service = new DEFRAShimTemperatureService(mockShimService.Object);

            var expected = new ReadingInfo()
            {
                LicenseInfo = DEFRAShimTemperatureService.LicenseString,
                Unit = DEFRAShimTemperatureService.Unit,
                Timestamp = fakeShimServiceResponse.Timestamp,
                Station = TestMetadata.ToStation(),
                Value = fakeShimServiceResponse.Temperature.Value
            };

            var actual = await service.GetTemperatureInfo(TestMetadata, TestDateTime);

            actual.Should().BeEquivalentTo(expected);

        }


        [Fact]
        public async void DEFRAShimTemperatureService_Get_ReturnsNullIfShimReturnsNull()
        {

            var mockShimService = new Mock<IDEFRAShimService>();
            mockShimService.Setup(x => x.GetDataFromShim(TestMetadata, TestDateTime)).ReturnsAsync((ShimResponse?)null);

            var service = new DEFRAShimTemperatureService(mockShimService.Object);


            var actual = await service.GetTemperatureInfo(TestMetadata, TestDateTime);

            actual.Should().BeNull();

        }

        private static float ValidTemperatureReading = 1.0f;
    }
}
