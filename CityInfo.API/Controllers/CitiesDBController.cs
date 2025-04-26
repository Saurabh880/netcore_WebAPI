using CityInfo.API.Interface_Repo;
using CityInfo.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/citiesdb")]
    public class CitiesDBController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;

        public CitiesDBController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }
        [HttpGet()]
        public  async Task<ActionResult<IEnumerable<CityWithoutPointsOfInteresDto>>> GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();

            var results = new List<CityWithoutPointsOfInteresDto>();
            // mapping the City entities to CityWithoutPointsOfInterestDto's
            foreach (var cityEntity in cityEntities) {
                results.Add(new CityWithoutPointsOfInteresDto
                {
                    Id = cityEntity.Id,
                    Name = cityEntity.Name,
                    Description = cityEntity.Description
                });
            }
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id , bool includePointsOfInterest = false)
        {
            var cityEntity = await _cityInfoRepository.GetCity(id, includePointsOfInterest);
           
            if (cityEntity == null)
            {   
                return NotFound();
            }
            var cityWithoutPOI = new CityWithoutPointsOfInteresDto
            {
                Id = cityEntity.Id,
                Name = cityEntity.Name,
                Description = cityEntity.Description
            };
            if (includePointsOfInterest)
            {
                var cityWithPOI = new CityDto
                {
                    Id = cityEntity.Id,
                    Name = cityEntity.Name,
                    Description = cityEntity.Description
                };
                foreach (var poi in cityEntity.PointsOfInterest)
                {
                    var pointOfInterestDto = new PointOfInterestDto
                    {
                        Id = poi.Id,
                        Name = poi.Name,
                        Description = poi.Description
                    };
                    if (string.IsNullOrEmpty(pointOfInterestDto.Description))
                    {
                        pointOfInterestDto.Description = "No description available";
                    }
                    cityWithPOI.PointsOfInterest.Add(pointOfInterestDto);
                }

                return Ok(cityWithPOI);
            }
            return Ok(cityWithoutPOI);            
        }
    }
}
