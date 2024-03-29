﻿using AutoMapper;
using HotelListing.Core.DTOs;
using HotelListing.Core.Models;
using HotelListing.Core.Services;
using HotelListing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> userManager;
        private readonly ILogger<AccountController> logger;
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public AccountController(UserManager<ApiUser> userManager, ILogger<AccountController> logger, IMapper mapper, IAuthManager authManager)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.mapper = mapper;
            this.authManager = authManager;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDTO userDTO)
        {
            logger.LogInformation($"Registration attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = mapper.Map<ApiUser>(userDTO);
            user.UserName = userDTO.Email;
            var result = await userManager.CreateAsync(user, userDTO.Password);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(error.Code, error.Description);
                });

                return BadRequest(ModelState);
            }
            await userManager.AddToRolesAsync(user, userDTO.Roles);

            return Accepted();
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDTO userDTO)
        {
            logger.LogInformation($"Login attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await authManager.ValidateUserAsync(userDTO))
            {
                return Unauthorized();
            }

            return Accepted(new TokenRequest { Token = await authManager.CreateTokenAsync(), RefreshToken = await authManager.CreateRefreshTokenAsync() });
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            var result = await authManager.VerifyRefreshTokenAsync(tokenRequest);
            if (result is null)
            {
                return Unauthorized();
            }

            return Ok(tokenRequest);
        }
    }
}
