using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using B2B_Subscription.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database Contexts Configuration
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));

builder.Services.AddDbContext<SubscriptionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SubscriptionConnection")));

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentConnection")));

builder.Services.AddDbContext<LicenseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LicenseConnection")));

// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Identity Configuration (if using authentication)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>();

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

// API endpoints will go here
app.MapControllers();

app.Run(); 