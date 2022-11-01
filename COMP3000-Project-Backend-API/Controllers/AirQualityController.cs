using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models;
using Microsoft.AspNetCore.Mvc;

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
}