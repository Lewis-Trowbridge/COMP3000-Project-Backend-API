using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models;
using Microsoft.AspNetCore.Mvc;
using COMP3000_Project_Backend_API.Models.Request;

namespace COMP3000_Project_Backend_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirQualityController: ControllerBase {
    private readonly IMetadataService _metadataService;
    private readonly IAirQualityService _airQualityService;

    public AirQualityController(IMetadataService metadataService, IAirQualityService airQualityService)
    {
        _metadataService = metadataService;
        _airQualityService = airQualityService;
    }

    [HttpGet]
    public async Task<AirQualityInfo[]> GetAirQuality(AirQualityRequest request)
    {
        var stations = await _metadataService.GetAsync(request.Bbox!);
        var tasks = new List<Task<AirQualityInfo>>();

        foreach (var station in stations)
        {
            tasks.Add(_airQualityService.GetAirQualityInfo(station, request.Timestamp));
        }

        return await Task.WhenAll(tasks);
    }
}