using CityInfo.API.Entities;
using CityInfo.API.Interface_Repo;
using CityInfo.API.Model;
using CityInfo.API.Services;
using EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointofinterestdb")]
    [ApiController]
    public class PointsOfInterestDBController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly EmailConfiguration _mailService;
        private readonly IEmailSender _emailSender;
        private readonly ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestDBController(ILogger<PointsOfInterestController> logger, EmailConfiguration localMail, IEmailSender emailSender,
            ICityInfoRepository cityInfoRepository)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _mailService = localMail ??
                throw new ArgumentNullException(nameof(localMail));
            _emailSender = emailSender;
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            //HttpContext.RequestServices.GetService(typeof(ILogger<PointsOfInterestController>));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.IfCityExistsAsync(cityId)) {
                _logger.LogInformation(
                    $"City with Id {cityId} wasn't found when accessing the Points of Interest.");
                return NotFound ();
            }

            var poitnOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            var results = new List<PointOfInterestDto>();
            foreach (var poi in poitnOfInterestForCity)
            {
                results.Add(new PointOfInterestDto
                {
                    Id = poi.Id,
                    Name = poi.Name,
                    Description = poi.Description
                });

            }
            return Ok(results);
        }

        [HttpGet("{pointofinterestId}", Name = "GetPointOfInterestDb")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.IfCityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with Id {cityId} wasn't found when accessing the Points of Interest.");
                return NotFound();
            }
            //find point of interest
            var getpointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (getpointOfInterest == null)
            {
                return NotFound();
            }
            var PointOfInterest = new PointOfInterestDto
            {
                Id = getpointOfInterest.Id,
                Name = getpointOfInterest.Name,
                Description = getpointOfInterest.Description
            };

            return Ok(PointOfInterest);
        }
        [HttpPost]
        public async  Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityid, PointOfInterestCreationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.IfCityExistsAsync(cityid))
            {
                _logger.LogInformation(
                    $"City with Id {cityid} wasn't found when accessing the Points of Interest.");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //PointOfInterestCreationDto maps to PointIfinterest Entity
            var poiEntity = new PointOfInterest(pointOfInterest.Name)
            {
                Description = pointOfInterest.Description,
                CityId = cityid
            };


            var finalPointOfInterest = new PointOfInterestDto()
            {
                
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityid, poiEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityid, pointOfInterestId = finalPointOfInterest.Id }, finalPointOfInterest);
        }
        //[HttpPut("{pointOfInterestId}")]
        //public ActionResult UpdatePointOfInterest(int cityid, int pointOfInterestId, PointIfInterestUpdateDto pointOfInterestUpdate)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityid);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }
        //    var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        //    if (pointOfInterest == null)
        //    {
        //        return NotFound();
        //    }
        //    pointOfInterest.Name = pointOfInterestUpdate.Name;
        //    pointOfInterest.Description = pointOfInterestUpdate.Description;
        //    return NoContent();
        //}

        //[HttpPatch("{pointofInterestId}")]
        //public ActionResult PartiallyUpdatePointOfInterest(int cityid, int pointofInterestId, JsonPatchDocument<PointIfInterestUpdateDto> patchDocument)
        //{
        //    var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityid);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }
        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofInterestId);
        //    if (pointOfInterestFromStore == null)
        //    {
        //        return NotFound();
        //    }
        //    var pointOfInterestToPatch = new PointIfInterestUpdateDto()
        //    {
        //        Name = pointOfInterestFromStore.Name,
        //        Description = pointOfInterestFromStore.Description
        //    };
        //    //if we pass in the ModelState to the ApplyTo method, any errors like changing the property that doesn't exist , will make this ModelState invalid.
        //    patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    //This triggers validation of our model and any errors will end up in the ModelState. If an error happens, it returns false.
        //    if (!TryValidateModel(pointOfInterestToPatch))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        //    pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
        //    return NoContent();
        //}
        //[HttpDelete("{pointOfInterestId}")]
        //public ActionResult DeletePointOfInterest(int cityid, int pointOfInterestId)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityid);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }
        //    var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        //    if (pointOfInterest == null)
        //    {
        //        return NotFound();
        //    }
        //    city.PointsOfInterest.Remove(pointOfInterest);

        //    var message = new Message("loren.cummings28@ethereal.email", "Test email", $"Point of interest {pointOfInterest.Name} with id {pointOfInterestId} was deleted.");

        //    _emailSender.SendEmail(message);
        //    return NoContent();
        //}
    }
}
