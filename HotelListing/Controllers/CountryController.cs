using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOs;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
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

        [MapToApiVersion("1.0")]
        [HttpGet("WorldCountries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> WorldCountriesAsync([FromQuery] RequestParams requestParams)
        {
            var countries = await unitOfWork.Countries.GetAllAsync(requestParams);
            var worldCounties = mapper.Map<IList<WorldCountryDTO>>(countries);
            return Ok(worldCounties);
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> WorldCountryHotelsAsync([FromQuery] RequestParams requestParams)
        {
            var countries = await unitOfWork.Countries.GetAllAsync(requestParams);
            var response = mapper.Map<IList<CountryDTO>>(countries);
            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCountryByIdAsync(int id)
        {
            var country = await unitOfWork.Countries.GetAsync(c => c.Id == id);
            if (country is null)
            {
                return NotFound();
            }

            var response = mapper.Map<CountryDTO>(country);
            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountryAsync(int id, [FromBody] UpdateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                return BadRequest(ModelState.Select(x => x.Value.Errors).ToList());
            }

            var country = await unitOfWork.Countries.GetAsync(c => c.Id == id);
            if (country is null)
            {
                logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountryAsync)}");
                return BadRequest("Submitted data is invalid");
            }

            mapper.Map(source: countryDTO, destination: country);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
