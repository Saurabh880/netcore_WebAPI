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
        public ActionResult<CityDto> GetCity(int id)
        {
            //var cityToReturn = _citiesData.Cities.FirstOrDefault(c => c.Id == id);
            //if (cityToReturn == null)
            //{
            //    return NotFound();
            //}
            return Ok();
        }
    }
}
