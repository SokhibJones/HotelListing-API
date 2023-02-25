using AutoMapper;
using HotelListing.DTOs;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CountryController> logger;
        private readonly IMapper mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpGet("WorldCountries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> WorldCountriesAsync()
        {
            try
            {
                var countries = await unitOfWork.Countries.GetAllAsync();
                var worldCounties = mapper.Map<IList<WorldCountryDTO>>(countries);
                return Ok(worldCounties);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(WorldCountriesAsync)}: {Environment.NewLine}{ex.Message}");
                return StatusCode(500, "Internal Server Error. Please try later.");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> WorldCountryHotelsAsync()
        {
            try
            {
                var countries = await unitOfWork.Countries.GetAllAsync();
                var response = mapper.Map<IList<CountryDTO>>(countries);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(WorldCountryHotelsAsync)}: {Environment.NewLine}{ex.Message}");
                return StatusCode(500, "Internal Server Error. Please try later.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCountryByIdAsync(int id)
        {
            try
            {
                var country = await unitOfWork.Countries.GetAsync(c => c.Id == id);
                if (country is null)
                {
                    return NotFound();
                }

                var response = mapper.Map<CountryDTO>(country);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetCountryByIdAsync)}: {Environment.NewLine}{ex.Message}");
                return StatusCode(501, "Internal Server Error. Please try later.");
            }
        }

    }
}
