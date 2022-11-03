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
    }
}
