using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace COMP3000_Project_Backend_API.Models.MongoDB;

public class DEFRAMetadata {
    [BsonId]
    public string? Id { get; set; }
    public string SiteName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float[] Coords { get; set; } = null!;
}