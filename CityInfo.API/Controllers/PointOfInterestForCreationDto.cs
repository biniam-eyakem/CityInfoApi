using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Controllers
{
    public class PointOfInterestForCreationDto
    {
        [Required(ErrorMessage = "Point of interest must have a name. Name field is required.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
