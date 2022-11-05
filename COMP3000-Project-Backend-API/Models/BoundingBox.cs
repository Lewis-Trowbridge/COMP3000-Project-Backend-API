using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace COMP3000_Project_Backend_API.Models
{
    public class BoundingBox
    {
        public BoundingBox()
        {

        }

        public BoundingBox(double bottomLeftX, double bottomLeftY, double topRightX, double topRightY)
        {
            BottomLeftX = bottomLeftX;
            BottomLeftY = bottomLeftY;
            TopRightX = topRightX;
            TopRightY = topRightY;
        }

        [Required]
        public double BottomLeftX { get; set; }
        [Required]
        public double BottomLeftY { get; set; }
        [Required]
        public double TopRightX { get; set; }
        [Required]
        public double TopRightY { get; set; }
    }
}
