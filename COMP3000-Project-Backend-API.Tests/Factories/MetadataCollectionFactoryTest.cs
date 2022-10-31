using COMP3000_Project_Backend_API.Factories;
using COMP3000_Project_Backend_API.Models.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace COMP3000_Project_Backend_API.Tests.Factories
{
    public class MetadataCollectionFactoryTest
    {
        [Fact]
        public void MetadataCollectionFactory_GetMongoCollection_CreatesCorrectCollection()
        {
            var testServerHost = "mongodb0.example.com";
            var testServerPort = 27017;
            var testConnectionString = $"mongodb://{testServerHost}:{testServerPort}";
            var testDatabaseName = "database";
            var testCollectionName = "collection";

            var testOptions = Options.Create(new MongoDBSettings()
            {
                ConnectionString = testConnectionString,
                DatabaseName = testDatabaseName,
                CollectionName = testCollectionName,
            });

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(typeof(IOptions<MongoDBSettings>))).Returns(testOptions);

            var actual = MetadataCollectionFactory.GetMongoCollection(mockServiceProvider.Object);

            actual.CollectionNamespace.CollectionName.Should().Be(testCollectionName);
            actual.Database.DatabaseNamespace.DatabaseName.Should().Be(testDatabaseName);
            actual.Database.Client.Settings.Server.Host.Should().Be(testServerHost);
            actual.Database.Client.Settings.Server.Port.Should().Be(testServerPort);
        }
    }
}
