using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PropertyOS.Infrastructure.Data;
using PropertyOS.Infrastructure.DependencyInjection;
using PropertyOS.Api.Middleware;
using PropertyOS.Api.Authorization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionAuthorizationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PropertyOsDbContext>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Supabase:JwtIssuer"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Supabase:JwtIssuer"],

            ValidateAudience = true,
            ValidAudience = "authenticated",

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ClockSkew = TimeSpan.FromSeconds(60)
        };
    });

var app = builder.Build();
app.Logger.LogInformation("PropertyOS API started successfully");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("WebApp");

app.UseAuthentication();
app.UseAuthorization();

// Must run after UseAuthentication()/UseAuthorization() — organization
// resolution (Sprint 2) will read claims off HttpContext.User, which is
// not populated until the authentication middleware has run.
app.UseMiddleware<OrganizationContextMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

/// <summary>
/// Entry point for the PropertyOS API application.
/// </summary>
public partial class Program { }