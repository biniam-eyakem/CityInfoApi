using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:ApiVersion}/cities/{cityId}/[controller]")]
    [Authorize(Policy = "MustBeFromAntwerp")]
    [ApiVersion("2.0")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            try
            {
                // use authenticated user's city from claims to control accress to resource (only matched allowed)
                var cityName = User.Claims.FirstOrDefault(u => u.Type == "city")?.Value;
                if (!(await _cityInfoRepository.CityNameMatchCityId(cityName, cityId)))
                {
                    return Forbid();
                }
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    this._logger.LogInformation($"City with {cityId} was't found when looking for points of interest.");
                    return NotFound();
                }

                var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
            }
            catch (Exception exception)
            {
                this._logger.LogCritical(
                    $"Exception while getting points of interest for city with id: ${cityId}", exception);
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }


        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
            int cityId,
            PointOfInterestForCreationDto pointOfInterestForCreate)
        {
            var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);
            if (!cityExists)
            {
                return NotFound();
            }


            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterestForCreate);
            await _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new { cityId, pointofinterestId = createdPointOfInterest.Id },
                createdPointOfInterest);
        }

        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfIntetest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto updatePointOfInterest)
        {
            var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);
            if (!cityExists)
            {
                return NotFound();
            }

            var pointOfInteresstEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInteresstEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(updatePointOfInterest, pointOfInteresstEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestForUpdateDto> pointOfInterestPatchDocument)
        {
            var cityExist = await _cityInfoRepository.CityExistsAsync(cityId);
            if (!cityExist)
            {
                return NotFound();
            }

            var existingPointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (existingPointOfInterest == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(existingPointOfInterest);
            pointOfInterestPatchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, existingPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> Delete(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();
            this._mailService.Send(
                "Point of interest deleted",
                $"Point of interest {pointOfInterest.Name} with id {pointOfInterest.Id} was deleted.");
            return NoContent();
        }
    }
}
