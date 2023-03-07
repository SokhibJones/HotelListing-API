using System.ComponentModel.DataAnnotations;

namespace HotelListing.Core.DTOs
{
    public class UserRegisterDTO : UserLoginDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public ICollection<string> Roles { get; set; }
    }
}
