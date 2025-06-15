namespace CityInfo.API.Model
{
    /// <summary>
    /// A city DTO without points of interest.
    /// </summary>
    public class CityWithoutPointsOfInteresDto
    {
        /// <summary>
        /// The id of city
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the city.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The description of the city.
        /// </summary>
        public string? Description { get; set; }
    }
}
