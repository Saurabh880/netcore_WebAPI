using CityInfo.API.Interface_Repo;
using CityInfo.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/citiesdb")]
    public class CitiesDBController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        const int maxCitiesPage = 20;

        public CitiesDBController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }
        [HttpGet()]
        public  async Task<ActionResult<IEnumerable<CityWithoutPointsOfInteresDto>>> GetCities( string? name, string? searchQuery, int pageNumber=1, int pageSize=10)
        {
            if(pageSize > maxCitiesPage)
            {
                pageSize = maxCitiesPage;
            }

            var (cityEntities, paginationMetadata )= await _cityInfoRepository.GetCitiesAsync(name,searchQuery, pageNumber,pageSize);
            //var cityEntitiesFiltered = await _cityInfoRepository.GetCitiesAsync(name,null );


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
            //paginationMetadata as a header to our response.
            //we pass through a name for the header, X‑Pagination, and a value, which, in our case, will be a Serialized version of paginationMetadata
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

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
