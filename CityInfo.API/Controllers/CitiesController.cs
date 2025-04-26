using CityInfo.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly CitiesDataStore _citiesData;

        public CitiesController(CitiesDataStore citiesData)
        {
            _citiesData = citiesData;
        }
        [HttpGet()]
        public  ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(_citiesData.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var cityToReturn = _citiesData.Cities.FirstOrDefault(c => c.Id == id);
            if (cityToReturn == null)
            {
                return NotFound();
            }
            return Ok(cityToReturn);
        }
    }
}
