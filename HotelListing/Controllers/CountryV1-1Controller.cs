using HotelListing.Core.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [ApiVersion("1.1")]
    [Route("api/Country")]
    [ApiController]
    public class CountryV1_1Controller : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public CountryV1_1Controller(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [MapToApiVersion("1.1")]
        [HttpGet("WorldCountries")]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await unitOfWork.Countries.GetAllAsync();
            return Ok(countries);
        }
    }
}
