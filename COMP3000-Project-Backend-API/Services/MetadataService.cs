using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using MongoDB.Driver;

namespace COMP3000_Project_Backend_API.Services;

public class MetadataService : IMetadataService
{
    private readonly IMongoCollection<DEFRAMetadata> _metadataCollection;

    public MetadataService(IMongoCollection<DEFRAMetadata> metadataCollection)
    {
        _metadataCollection = metadataCollection;
    }

    public async Task<List<DEFRAMetadata>> GetAsync(BoundingBox bbox)
    {
        var filterBuilder = Builders<DEFRAMetadata>.Filter;
        // Reverse the order as Mongo requires longitude, latitude
        var filter = filterBuilder.GeoWithinBox(x => x.Coords, bbox.BottomLeftY, bbox.BottomLeftX, bbox.TopRightY, bbox.TopRightX);
        return await (await _metadataCollection.FindAsync(filter)).ToListAsync();
    }
}
