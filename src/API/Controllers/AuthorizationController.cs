using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using B2B_Subscription.Core.Entities;
using B2B_Subscription.Infrastructure.Data.Repositories.User;
using B2B_Subscription.Core.DTOs;
using Newtonsoft.Json;


public class AuthorizationController : ControllerBase
{
    // Implement the Resource Owner Password Credentials flow for local development
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUserRepository _applicationUserRepository;

    public AuthorizationController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IApplicationUserRepository applicationUserRepository
    )   
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _applicationUserRepository = applicationUserRepository;
    }

    [HttpPost("connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("The specified grant type is not supported.");
        if (request.IsPasswordGrantType())
        {
            var user = await _userManager.FindByNameAsync(request.Username)
                ?? throw new InvalidOperationException("The username or password is invalid.");

            if (user == null)
            {
                return BadRequest(new OpenIddictResponse
                {
                    Error = "invalid_grant",
                    ErrorDescription = "The username or password is invalid."
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return BadRequest(new OpenIddictResponse
                {
                    Error = "invalid_grant",
                    ErrorDescription = "The username or password is invalid."
                });
            }

            // Create the claims-based identity that will be used by OpenIddict to generate tokens.
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            // Add the claims that will be persisted in the tokens.
            identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                    .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                    .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                    .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user));
                    // .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))]);
            
            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new InvalidOperationException("The specified grant type is not supported.");
    }

    [HttpGet]
    [Route("/api/stripe-customer-id/{userId}")]
    public async Task<IActionResult> GetStripeCustomerId(string userId)
    {
        var applicationUser = await _applicationUserRepository.GetApplicationUserByIdAsync(userId);
        if (applicationUser == null)
        {
            return NotFound();
        }
        var response = new {
            applicationUser.StripeCustomerId
        };
        return Ok(response);
    }

    [HttpPut]
    [Route("/api/stripe-customer-id/{userId}")]
    public async Task<IActionResult> UpdateStripeCustomerId(string userId, [FromBody] ApplicationUserDto applicationUserDto)
    {
        Console.WriteLine("Updating stripe customer id for user {0}", userId);

        if (applicationUserDto == null)
        {
            Console.WriteLine("ApplicationUserDto is null");
            return BadRequest("Request body is required");
        }
        
        Console.WriteLine("Received DTO: {0}", JsonConvert.SerializeObject(applicationUserDto));
        Console.WriteLine("Stripe customer id: {0}", applicationUserDto.StripeCustomerId ?? "null");
    
        var applicationUser = await _applicationUserRepository.GetApplicationUserByIdAsync(userId);
        if (applicationUser == null)
        {
            return NotFound();
        }
        applicationUser.StripeCustomerId = applicationUserDto.StripeCustomerId;
        await _applicationUserRepository.UpdateApplicationUserAsync(applicationUser);
        return Ok();
    }
}