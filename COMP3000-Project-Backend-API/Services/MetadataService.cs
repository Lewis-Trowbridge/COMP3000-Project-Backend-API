using COMP3000_Project_Backend_API.Models.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace COMP3000_Project_Backend_API.Services;

public class MetadataService {
    private readonly IMongoCollection<DEFRAMetadata> _metadataCollection;

    public MetadataService(IOptions<MongoDBSettings> mongoDbSettings) {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

        _metadataCollection = mongoDatabase.GetCollection<DEFRAMetadata>(mongoDbSettings.Value.CollectionName);
    }

    public async Task<List<DEFRAMetadata>> GetAsync() =>
        await _metadataCollection.Find(_ => true).ToListAsync();
}