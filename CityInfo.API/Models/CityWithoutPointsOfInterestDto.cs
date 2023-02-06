namespace CityInfo.API.Models
{
    /// <summary>
    /// A DTO for city without its points of interests
    /// </summary>
    public class CityWithoutPointsOfInterestDto
    {
        /// <summary>
        /// The <code ref="int">Id</code> of the city
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the city
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Description of the city
        /// </summary>
        public string? Description { get; set; }
    }
}
