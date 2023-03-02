using System.ComponentModel.DataAnnotations;

namespace HotelListing.DTOs
{
    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }
        public WorldCountryDTO Country { get; set; }
    }

    public class CreateHotelDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Hotel name is too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Address is too long")]
        public string Address { get; set; }

        [Required]
        [Range(1, 5)]
        public double Rating { get; set; }

        [Required]
        public int CountryId { get; set; }
    }

    public class UpdateHotelDTO : CreateHotelDTO
    {

    }

    public class UpsertHotelDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "Hotel name is too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Address is too long")]
        public string Address { get; set; }

        [Required]
        [Range(1, 5)]
        public double Rating { get; set; }

        public int CountryId { get; set; }
    }

    public class CoutryHotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }
    }
}
