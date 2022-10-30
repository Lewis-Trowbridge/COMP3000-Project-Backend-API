using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class MetadataServiceTest
    {

        [Fact]
        public async void MetadataService_GetAsync_ConstructsFilterCorrectly()
        {
            var mockCollection = new Mock<IMongoCollection<DEFRAMetadata>>();
            var mockAsyncCursor = new Mock<IAsyncCursor<DEFRAMetadata>>();

            var testBbox = new BoundingBox(0, 0, 0, 0);
            var expectedFilter = Builders<DEFRAMetadata>.Filter.GeoWithinBox(x => x.Coords, 0, 0, 0, 0);

            mockAsyncCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
            mockAsyncCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
            mockAsyncCursor.SetupGet(x => x.Current).Returns(new List<DEFRAMetadata>());

            mockCollection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<DEFRAMetadata>>(), It.IsAny<FindOptions<DEFRAMetadata>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockAsyncCursor.Object);


            var service = new MetadataService(mockCollection.Object);

            await service.GetAsync(testBbox);

            mockCollection.Verify(x => x.FindAsync(It.Is<FilterDefinition<DEFRAMetadata>>(actual => actual.ToJson(typeof(FilterDefinition<DEFRAMetadata>), default, default, default, default) == expectedFilter.ToJson(typeof(FilterDefinition<DEFRAMetadata>), default, default, default, default)), It.IsAny<FindOptions<DEFRAMetadata>>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
