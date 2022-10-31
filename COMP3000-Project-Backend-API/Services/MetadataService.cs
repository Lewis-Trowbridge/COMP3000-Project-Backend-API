using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using MongoDB.Driver;

namespace COMP3000_Project_Backend_API.Services;

public class MetadataService {
    private readonly IMongoCollection<DEFRAMetadata> _metadataCollection;

    public MetadataService(IMongoCollection<DEFRAMetadata> metadataCollection) {
        _metadataCollection = metadataCollection;
    }

    public async Task<List<DEFRAMetadata>> GetAsync(BoundingBox bbox)
    {
        var filterBuilder = Builders<DEFRAMetadata>.Filter;
        var filter = filterBuilder.GeoWithinBox(x => x.Coords, bbox.BottomLeftX, bbox.BottomLeftY, bbox.TopRightX, bbox.TopRightY);
        return await (await _metadataCollection.FindAsync(filter)).ToListAsync();
    }
}