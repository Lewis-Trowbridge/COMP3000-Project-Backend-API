using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace COMP3000_Project_Backend_API.Models.MongoDB;

public class DEFRAMetadata
{
    [BsonId]
    public string? Id { get; set; }
    [BsonElement("site_name")]
    public string SiteName { get; set; } = null!;
    [BsonElement("start_date")]
    public DateTime StartDate { get; set; }
    [BsonElement("end_date")]
    public DateTime EndDate { get; set; }
    [BsonElement("coords")]
    public double[] Coords { get; set; } = null!;
}
