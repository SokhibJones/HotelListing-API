using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOs;
using HotelListing.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AllHotelsAsync()
        {
            try
            {
                var hotels = await unitOfWork.Hotels.GetAllAsync();
                var response = mapper.Map<IList<HotelDTO>>(hotels);

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(AllHotelsAsync)}: {Environment.NewLine}{ex.Message}");
                return StatusCode(500, "Internal Server Error. Please try later.");
            }
        }

        [HttpGet("{id:int}", Name = "GetHotelById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotelByIdAsync(int id)
        {
            try
            {
                var hotel = await unitOfWork.Hotels.GetAsync(h => h.Id == id);
                if (hotel is null)
                {
                    return NotFound();
                }

                var response = mapper.Map<HotelDTO>(hotel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something went wrong in the {nameof(GetHotelByIdAsync)}: {Environment.NewLine}{ex.Message}");
                return StatusCode(500, "Internal Server Error. Please try later.");
            }
        }

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

            try
            {
                var hotel = mapper.Map<Hotel>(hotelDTO);
                await unitOfWork.Hotels.Insert(hotel);
                await unitOfWork.SaveAsync();

                return CreatedAtRoute("GetHotelById", new { id = hotel.Id }, hotel);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, $"Something went wrong in the {nameof(CreateHotelAsync)}: {Environment.NewLine}{exc.Message}");
                return StatusCode(500, "Internal Server Error. Please try later.");
            }
        }

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

            try
            {
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
            catch (Exception exc)
            {
                logger.LogError(exc, $"Something went wrong in the {nameof(UpdateHotelAsync)}: {Environment.NewLine}{exc.Message}");
                return StatusCode(500, "Internal Server Error. Please try later.");
            }
        }

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

            try
            {
                var hotel = await unitOfWork.Hotels.GetAsync(h => h.Id == id);
                if (hotel is null)
                {
                    return BadRequest("Submitted data is invalid");
                }

                await unitOfWork.Hotels.Delete(id);
                await unitOfWork.SaveAsync();

                return NoContent();
            }
            catch (Exception exc)
            {
                logger.LogError(exc, $"Something went wrong in the {nameof(DeleteHotelAsync)}: {Environment.NewLine}{exc.Message}");
                return StatusCode(500, "Internal Server Error. Please try later.");
            }
        }
    }
}
