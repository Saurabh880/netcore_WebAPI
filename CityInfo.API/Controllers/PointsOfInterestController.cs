using CityInfo.API.Model;
using CityInfo.API.Services;
using EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly EmailConfiguration _mailService;
        private readonly IEmailSender _emailSender;
        private readonly CitiesDataStore _citiesData;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, EmailConfiguration localMail, IEmailSender emailSender,CitiesDataStore citiesData)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           _mailService = localMail ?? throw new ArgumentNullException(nameof(localMail));
            _emailSender = emailSender;
            _citiesData = citiesData ?? throw new ArgumentNullException(nameof(citiesData));
            //HttpContext.RequestServices.GetService(typeof(ILogger<PointsOfInterestController>));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointsOfInterestController>> GetPointsOfInterest(int cityId)
        {
            //throw new Exception("Exception sample");
            try
            {
                
                var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                    return NotFound();
                }
                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);

                //as we are handling the exception, be it simply by logging it, so we must return this manually.
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{pointofinterestId}",Name = "GetPointOfInterest") ]
        public ActionResult<PointsOfInterestController> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
               
                return NotFound();
            }
            //find point of interest
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            return Ok(pointOfInterest);
        }
        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityid, PointOfInterestCreationDto pointOfInterest)
        {
            var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityid);
            if (city == null)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //find max id
            var maxPointOfInterestId = _citiesData.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };
            city.PointsOfInterest.Add(finalPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityid, pointOfInterestId = finalPointOfInterest.Id }, finalPointOfInterest);
        }
        [HttpPut("{pointOfInterestId}")]
        public ActionResult UpdatePointOfInterest(int cityid, int pointOfInterestId,PointIfInterestUpdateDto pointOfInterestUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityid);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            pointOfInterest.Name = pointOfInterestUpdate.Name;
            pointOfInterest.Description = pointOfInterestUpdate.Description;
            return NoContent();
        }

        [HttpPatch("{pointofInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityid,int pointofInterestId, JsonPatchDocument<PointIfInterestUpdateDto> patchDocument)
        {
            var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityid);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofInterestId);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }
            var pointOfInterestToPatch = new PointIfInterestUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };
            //if we pass in the ModelState to the ApplyTo method, any errors like changing the property that doesn't exist , will make this ModelState invalid.
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //This triggers validation of our model and any errors will end up in the ModelState. If an error happens, it returns false.
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
            return NoContent();
        }
        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityid, int pointOfInterestId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var city = _citiesData.Cities.FirstOrDefault(c => c.Id == cityid);
            if (city == null)
            {
                return NotFound();
            }
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }
            city.PointsOfInterest.Remove(pointOfInterest);
            
            var message = new Message("loren.cummings28@ethereal.email" , "Test email", $"Point of interest {pointOfInterest.Name} with id {pointOfInterestId} was deleted.");

            _emailSender.SendEmail(message);
            return NoContent();
        }
    }
}
