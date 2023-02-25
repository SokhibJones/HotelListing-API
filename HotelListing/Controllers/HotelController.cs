using AutoMapper;
using HotelListing.DTOs;
using HotelListing.IRepository;
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

        [HttpGet("{id:int}")]
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
    }
}
