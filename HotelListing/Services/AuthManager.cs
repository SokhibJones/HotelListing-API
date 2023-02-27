using HotelListing.Data;
using HotelListing.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApiUser> userManager;
        private readonly IConfiguration configuration;
        private ApiUser user;

        public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task<string> CreateTokenAsync()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaimsAsync();
            var token = GenerateToken(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GenerateToken(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var jwtToken = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: signingCredentials
            );

            return jwtToken;
        }

        private async Task<List<Claim>> GetClaimsAsync()
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            roles.ToList().ForEach(role =>
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            });

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("KEY");
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<bool> ValidateUserAsync(UserLoginDTO userDTO)
        {
            user = await userManager.FindByNameAsync(userDTO.Email);
            return user is not null && await userManager.CheckPasswordAsync(user, userDTO.Password);
        }
    }
}
