using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models;
using Microsoft.AspNetCore.Mvc;
using COMP3000_Project_Backend_API.Models.Request;

namespace COMP3000_Project_Backend_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirQualityController: ControllerBase {
    private readonly MetadataService _metadataService;
    private readonly DEFRACsvService _defraCsvService;

    public AirQualityController(MetadataService metadataService, DEFRACsvService defraCsvService)
    {
        _metadataService = metadataService;
        _defraCsvService = defraCsvService;
    }

    [HttpGet]
    public async Task<List<AirQualityInfo>> GetAirQuality(AirQualityRequest request)
    {
        var stations = await _metadataService.GetAsync(request.Bbox!);

        return new List<AirQualityInfo>();
    }
}