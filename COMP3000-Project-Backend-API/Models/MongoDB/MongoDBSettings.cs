namespace COMP3000_Project_Backend_API.Models.MongoDB;

public class MongoDBSettings {
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}