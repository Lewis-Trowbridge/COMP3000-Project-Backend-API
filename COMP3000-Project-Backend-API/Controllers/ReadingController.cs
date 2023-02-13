﻿using COMP3000_Project_Backend_API.Factories;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models.Request;
using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using SimpleDateTimeProvider;

namespace COMP3000_Project_Backend_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingController : ControllerBase
{
    private readonly IMetadataService _metadataService;
    private readonly IAirQualityServiceFactory _airQualityServiceFactory;

    public ReadingController(IMetadataService metadataService, IAirQualityServiceFactory airQualityServiceFactory)
    {
        _metadataService = metadataService;
        _airQualityServiceFactory = airQualityServiceFactory;
    }

    [HttpGet("/airquality")]
    public async Task<ReadingInfo[]> GetAirQuality([FromQuery] ReadingRequest request)
    {
        var service = _airQualityServiceFactory.GetAirQualityService(request.Timestamp);
        var stations = await _metadataService.GetAsync(request.Bbox!);
        var tasks = new List<Task<ReadingInfo?>>();

        foreach (var station in stations)
        {
            tasks.Add(service.GetAirQualityInfo(station, request.Timestamp));
        }

        return (await Task.WhenAll(tasks))
            .Where(x => x is not null)
            .ToArray()!;
    }

    [HttpGet("/temperature")]
    public async Task<ReadingInfo[]> GetTemperature([FromQuery] ReadingRequest request)
    {
        throw new NotImplementedException();
    }
}