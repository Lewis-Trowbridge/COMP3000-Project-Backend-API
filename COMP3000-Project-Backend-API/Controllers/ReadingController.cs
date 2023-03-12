using COMP3000_Project_Backend_API.Factories;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.Request;
using COMP3000_Project_Backend_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace COMP3000_Project_Backend_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingController : ControllerBase
{
    private readonly IMetadataService _metadataService;
    private readonly IReadingServiceFactory _readingServiceFactory;

    public ReadingController(IMetadataService metadataService, IReadingServiceFactory readingServiceFactory)
    {
        _metadataService = metadataService;
        _readingServiceFactory = readingServiceFactory;
    }

    [HttpGet("/airquality")]
    public async Task<ReadingInfo[]> GetAirQuality([FromQuery] ReadingRequest request)
    {
        var service = _readingServiceFactory.GetAirQualityService(request.Timestamp);
        var stations = await _metadataService.GetAsync(request.Bbox!);

        var tasks = stations.Select(station => service.GetAirQualityInfo(station, request.Timestamp));

        return (await Task.WhenAll(tasks))
            .Where(x => x is not null)
            .ToArray()!;
    }

    [HttpGet("/temperature")]
    public async Task<ReadingInfo[]> GetTemperature([FromQuery] ReadingRequest request)
    {
        var service = _readingServiceFactory.GetTemperatureService(request.Timestamp);
        var stations = await _metadataService.GetAsync(request.Bbox!);

        var tasks = stations.Select(station => service.GetTemperatureInfo(station, request.Timestamp));

        return (await Task.WhenAll(tasks))
            .Where(x => x is not null)
            .ToArray()!;
    }
}
