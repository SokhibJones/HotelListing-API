using HotelListing.Core.DTOs;
using HotelListing.Core.Models;

namespace HotelListing.Core.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUserAsync(UserLoginDTO userDTO);
        Task<string> CreateTokenAsync();
        Task<string> CreateRefreshTokenAsync();
        Task<TokenRequest> VerifyRefreshTokenAsync(TokenRequest tokenRequest);
    }
}
