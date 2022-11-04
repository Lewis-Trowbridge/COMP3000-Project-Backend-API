using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace COMP3000_Project_Backend_API.Models.Request
{
    public class AirQualityRequest
    {
        public BoundingBox? Bbox { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
