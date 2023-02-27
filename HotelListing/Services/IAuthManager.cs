using HotelListing.DTOs;

namespace HotelListing.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUserAsync(UserLoginDTO userDTO);
        Task<string> CreateTokenAsync();
    }
}
