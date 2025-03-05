using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2B_Subscription.Core.Entities;
using B2B_Subscription.Core.DTOs;
using B2B_Subscription.Infrastructure.Data.Repositories.User;
using System.Security.Claims;

namespace B2B_Subscription.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileController(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        // GET: api/UserProfile/me
        [HttpGet("me")]
        public async Task<ActionResult<UserProfile>> GetCurrentUserProfile()
        {
            // Get the current user's ID from the JWT claims
            var userId = User.FindFirst(OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject)?.Value;
            if (userId == null)
                return Unauthorized();

            var userProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(userId);
            if (userProfile == null)
                return NotFound();

            return Ok(userProfile);
        }

        // GET: api/UserProfile/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(Guid id)
        {
            var userProfile = await _userProfileRepository.GetUserProfileByIdAsync(id);
            if (userProfile == null)
                return NotFound();

            return Ok(userProfile);
        }

        // POST: api/UserProfile
        [HttpPost]
        public async Task<ActionResult<UserProfile>> CreateUserProfile(UserProfileDto createDto)
        {
            // Set the UserId from the JWT claim
            var userId = User.FindFirst(OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject)?.Value;
            if (userId == null)
                return Unauthorized();

            // Check if profile already exists
            var existingProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(userId);
            if (existingProfile != null)
                return Conflict("User profile already exists");

            var userProfile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FullName = createDto.FullName,
                Company = createDto.Company,
                PhoneNumber = createDto.PhoneNumber
            };
            var createdProfile = await _userProfileRepository.CreateUserProfileAsync(userProfile);
            return CreatedAtAction(nameof(GetUserProfile), new { id = createdProfile.Id }, createdProfile);
        }

        // PUT: api/UserProfile
        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile(UserProfileDto updateDto)
        {
            // Ensure user can only update their own profile
            var userId = User.FindFirst(OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject)?.Value;
            if (userId == null)
                return Unauthorized();

            var existingProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(userId);
            
            if (existingProfile == null)
                return NotFound();

            existingProfile.FullName = updateDto.FullName ?? existingProfile.FullName;
            existingProfile.Company = updateDto.Company ?? existingProfile.Company;
            existingProfile.PhoneNumber = updateDto.PhoneNumber ?? existingProfile.PhoneNumber;

            await _userProfileRepository.UpdateUserProfileAsync(existingProfile);
            return NoContent();
        }

        // DELETE: api/UserProfile
        [HttpDelete]
        public async Task<IActionResult> DeleteUserProfile()
        {
            // Ensure user can only delete their own profile
            var userId = User.FindFirst(OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject)?.Value;
            if (userId == null)
                return Unauthorized();

            var existingProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(userId);
            
            if (existingProfile == null)
                return NotFound();

            var result = await _userProfileRepository.DeleteUserProfileAsync(existingProfile.Id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}