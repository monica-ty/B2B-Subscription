using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using B2B_Subscription.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using B2B_Subscription.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenIddict.Validation.AspNetCore;
using B2B_Subscription.Infrastructure.Data.Repositories.User;

var builder = WebApplication.CreateBuilder(args);

// Database Contexts Configuration
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection"));
    options.UseOpenIddict();
});

builder.Services.AddDbContext<SubscriptionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SubscriptionConnection")));

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentConnection")));

builder.Services.AddDbContext<LicenseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LicenseConnection")));

// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

// Identity Configuration (if using authentication)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    // options.Tokens.AuthenticatorTokenProvider = null;
})
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

// Register OpenIddict
builder.Services.AddOpenIddict()
    .AddCore(options => {
        options.UseEntityFrameworkCore()
        .UseDbContext<UserDbContext>();
    })
    .AddServer(options => {
        // options.AllowAuthorizationCodeFlow()
        // .AllowRefreshTokenFlow()
        // .RequireProofKeyForCodeExchange();

        options.SetTokenEndpointUris("/connect/token");
        // options.SetAuthorizationEndpointUris("/connect/authorize");

        options.AllowPasswordFlow();

        // Accept anonymous clients (i.e, clients that don't send a client_id) for dev
        options.AcceptAnonymousClients();

        // Register the scopes supported by the server
        // options.RegisterScopes("api.read", "api.write");

        // Register the signing and encryption credentials
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        // Register the ASP.NET host and configure the ASP.NET options
        options.UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options => {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

    
// Authentication Configuration
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddHttpsRedirection(options =>{
    options.HttpsPort = 7043;
});
// Add a dummy email sender if you don't have real email functionality yet
builder.Services.AddTransient<IEmailSender<ApplicationUser>, NoOpEmailSender>();

var app = builder.Build();

// Development-specific middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Identity API
app.MapIdentityApi<ApplicationUser>();
// API endpoints will go here
app.MapControllers();

app.Run(); 