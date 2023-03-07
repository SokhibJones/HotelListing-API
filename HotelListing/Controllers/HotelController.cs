using AutoMapper;
using HotelListing.Core.DTOs;
using HotelListing.Core.IRepository;
using HotelListing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<HotelController> logger;
        private readonly IMapper mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AllHotelsAsync([FromQuery] RequestParams requestParams)
        {
            var hotels = await unitOfWork.Hotels.GetAllAsync(requestParams);
            var response = mapper.Map<IList<HotelDTO>>(hotels);

            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{id:int}", Name = "GetHotelById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelByIdAsync(int id)
        {
            var hotel = await unitOfWork.Hotels.GetAsync(h => h.Id == id);
            if (hotel is null)
            {
                return NotFound();
            }

            var response = mapper.Map<HotelDTO>(hotel);
            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotelAsync([FromBody] CreateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError($"Invalid POST request for {nameof(CreateHotelDTO)}");
                return BadRequest(hotelDTO);
            }

            var hotel = mapper.Map<Hotel>(hotelDTO);
            await unitOfWork.Hotels.Insert(hotel);
            await unitOfWork.SaveAsync();

            return CreatedAtRoute("GetHotelById", new { id = hotel.Id }, hotel);
        }

        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotelAsync(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotelAsync)}");
                return BadRequest(ModelState);
            }

            var hotel = await unitOfWork.Hotels.GetAsync(h => h.Id == id);
            if (hotel is null)
            {
                logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotelAsync)}");
                return BadRequest("Submitted data is invalid");
            }

            mapper.Map(source: hotelDTO, destination: hotel);
            await unitOfWork.SaveAsync();

            return NoContent();
        }

        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHotelAsync(int id)
        {
            if (id < 1)
            {
                logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotelAsync)}");
                return BadRequest();
            }

            var hotel = await unitOfWork.Hotels.GetAsync(h => h.Id == id);
            if (hotel is null)
            {
                return BadRequest("Submitted data is invalid");
            }

            await unitOfWork.Hotels.Delete(id);
            await unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
