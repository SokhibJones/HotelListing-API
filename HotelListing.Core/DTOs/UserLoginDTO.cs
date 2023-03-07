using System.ComponentModel.DataAnnotations;

namespace HotelListing.Core.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
