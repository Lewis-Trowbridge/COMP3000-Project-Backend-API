using COMP3000_Project_Backend_API.Models.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace COMP3000_Project_Backend_API.Factories
{
    public class MetadataCollectionFactory
    {

        public static IMongoCollection<DEFRAMetadata> GetMongoCollection(IServiceProvider serviceProvider)
        {

            var options = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>();
            var mongoClient = new MongoClient(options.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);

            return mongoDatabase.GetCollection<DEFRAMetadata>(options.Value.CollectionName);
        }
    }
}
