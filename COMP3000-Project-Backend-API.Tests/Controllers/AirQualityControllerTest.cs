using COMP3000_Project_Backend_API.Controllers;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models.Request;
using COMP3000_Project_Backend_API.Services;

namespace COMP3000_Project_Backend_API.Tests.Controllers
{
    public class AirQualityControllerTest
    {
        [Fact]
        public async void AirQualityController_GetAirQuality_CallsGetAirQualityInfoForEachMetadata()
        {
            var mockMetadataService = new Mock<IMetadataService>();
            var mockAirQualityService = new Mock<IAirQualityService>();
            var testBbox = new BoundingBox(0, 0, 0, 0);

            var metadata1 = new DEFRAMetadata();
            var metadata2 = new DEFRAMetadata();
            var metadata3 = new DEFRAMetadata();

            var metadataList = new List<DEFRAMetadata>()
            {
                metadata1,
                metadata2,
                metadata3
            };

            var testDatetime = DateTime.MinValue;

            var testAirQualityRequest = new AirQualityRequest()
            {
                Bbox = testBbox,
                Timestamp = testDatetime
            };

            mockMetadataService.Setup(x => x.GetAsync(testBbox)).ReturnsAsync(metadataList);

            mockAirQualityService.Setup(x => x.GetAirQualityInfo(It.IsAny<DEFRAMetadata>(), It.IsAny<DateTime>())).ReturnsAsync(new AirQualityInfo());

            var controller = new AirQualityController(mockMetadataService.Object, mockAirQualityService.Object);

            await controller.GetAirQuality(testAirQualityRequest);

            mockAirQualityService.Verify(x => x.GetAirQualityInfo(metadata1, testDatetime), Times.Once());
            mockAirQualityService.Verify(x => x.GetAirQualityInfo(metadata2, testDatetime), Times.Once());
            mockAirQualityService.Verify(x => x.GetAirQualityInfo(metadata3, testDatetime), Times.Once());
        }

        [Fact]
        public async void AirQualityController_GetAirQuality_ReturnsDataInCorrectFormat()
        {
            var mockMetadataService = new Mock<IMetadataService>();
            var mockAirQualityService = new Mock<IAirQualityService>();
            var testBbox = new BoundingBox(0, 0, 0, 0);

            var metadata1 = new DEFRAMetadata() { SiteName = "1" };
            var metadata2 = new DEFRAMetadata() { SiteName = "2" };
            var metadata3 = new DEFRAMetadata() { SiteName = "3" };

            var metadataList = new List<DEFRAMetadata>()
            {
                metadata1,
                metadata2,
                metadata3
            };

            var testDatetime = DateTime.MinValue;

            var testAirQualityRequest = new AirQualityRequest()
            {
                Bbox = testBbox,
                Timestamp = testDatetime
            };

            mockMetadataService.Setup(x => x.GetAsync(testBbox)).ReturnsAsync(metadataList);

            var airQuality1 = new AirQualityInfo() { Station = new Station() { Name = "1" } };
            var airQuality2 = new AirQualityInfo() { Station = new Station() { Name = "2" } };
            var airQuality3 = new AirQualityInfo() { Station = new Station() { Name = "3" } };
            var expected = new AirQualityInfo[] { airQuality1, airQuality2, airQuality3 };
            
            mockAirQualityService.Setup(x => x.GetAirQualityInfo(metadata1, It.IsAny<DateTime>())).ReturnsAsync(airQuality1);
            mockAirQualityService.Setup(x => x.GetAirQualityInfo(metadata2, It.IsAny<DateTime>())).ReturnsAsync(airQuality2);
            mockAirQualityService.Setup(x => x.GetAirQualityInfo(metadata3, It.IsAny<DateTime>())).ReturnsAsync(airQuality3);


            var controller = new AirQualityController(mockMetadataService.Object, mockAirQualityService.Object);

            var actual = await controller.GetAirQuality(testAirQualityRequest);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
