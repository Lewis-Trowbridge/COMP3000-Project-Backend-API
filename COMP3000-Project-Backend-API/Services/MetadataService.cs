using COMP3000_Project_Backend_API.Models.MongoDB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace COMP3000_Project_Backend_API.Services;

public class MetadataService {
    private readonly IMongoCollection<DEFRAMetadata> _metadataCollection;

    public MetadataService(IMongoCollection<DEFRAMetadata> metadataCollection) {
        _metadataCollection = metadataCollection;
    }

    public async Task<List<DEFRAMetadata>> GetAsync() =>
        await _metadataCollection.Find(_ => true).ToListAsync();
}