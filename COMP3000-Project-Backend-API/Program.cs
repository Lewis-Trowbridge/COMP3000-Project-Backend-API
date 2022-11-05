using COMP3000_Project_Backend_API.Factories;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("Mongo"));
builder.Services.AddSingleton<IMongoCollection<DEFRAMetadata>>(MetadataCollectionFactory.GetMongoCollection);
builder.Services.AddSingleton<IMetadataService, MetadataService>();

builder.Services.AddHttpClient<IAirQualityService, DEFRACsvService>(client =>
{
    client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }