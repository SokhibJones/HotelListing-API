using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOs
{
    public class WorldCountryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; set; }
        public IList<CoutryHotelDTO> Hotels { get; set; }
    }

    public class CreateCountryDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Country name is too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "Country code is too long")]
        public string Code { get; set; }
    }

    public class UpdateCountryDTO : CreateCountryDTO
    {
        public IList<UpsertHotelDTO> Hotels { get; set; }
    }
}
