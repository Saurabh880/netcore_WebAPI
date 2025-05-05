using CityInfo.API.Entities;
using CityInfo.API.Interface_Repo;
using CityInfo.API.Model;
using CityInfo.API.Services;
using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointofinterestdb")]
    [Authorize]
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
                       

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityid, poiEntity);
            await _cityInfoRepository.SaveChangesAsync();

            //mapping the entity back to a DTO before returning 
            var createdPoiToReturn = new PointOfInterestDto
            {
                Id = poiEntity.Id,
                Name = poiEntity.Name,
                Description = poiEntity.Description
            };

            return CreatedAtRoute("GetPointOfInterestDb", new { cityId = cityid, pointOfInterestId = createdPoiToReturn.Id }, createdPoiToReturn);
        }
        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityid, int pointOfInterestId, PointIfInterestUpdateDto pointOfInterestUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _cityInfoRepository.IfCityExistsAsync(cityid))
            {
                return NotFound();
            }
            //find Point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityid,pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            pointOfInterestEntity.Name = pointOfInterestUpdate.Name;
            pointOfInterestEntity.Description = pointOfInterestUpdate.Description;
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{pointofInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityid, int pointofInterestId, JsonPatchDocument<PointIfInterestUpdateDto> patchDocument)
        {
            // Get the existing entity from DB
            if (!await _cityInfoRepository.IfCityExistsAsync(cityid))
            {
                return NotFound();
            }
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityid, pointofInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            // Manually create a DTO from the existing entity
            var poiToPatch = new PointIfInterestUpdateDto
            {
                Name = pointOfInterestEntity.Name,
                Description = pointOfInterestEntity.Description
            };
            //if we pass in the ModelState to the ApplyTo method, any errors like changing the property that doesn't exist , will make this ModelState invalid.
            patchDocument.ApplyTo(poiToPatch, ModelState);

            // Validate after applying patch
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //This triggers validation of our model and any errors will end up in the ModelState. If an error happens, it returns false.
            if (!TryValidateModel(poiToPatch))
            {
                return BadRequest(ModelState);
            }

            // Manually map the patched DTO back to the Entity
            pointOfInterestEntity.Name = poiToPatch.Name;
            pointOfInterestEntity.Description = poiToPatch.Description;

            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityid, int pointOfInterestId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _cityInfoRepository.IfCityExistsAsync(cityid))
            {
                return NotFound();
            }
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityid, pointOfInterestId );
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            var message = new Message("loren.cummings28@ethereal.email", "Test email", $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestId} was deleted.");

            _emailSender.SendEmail(message);
            return NoContent();
        }
    }
}
