using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Model
{
    public class PointIfInterestUpdateDto
    {
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(200, ErrorMessage = "The text limit is 200 characters")]
        public string? Description { get; set; }
    }
}
