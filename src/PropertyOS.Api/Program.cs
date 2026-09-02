using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
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

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebApp", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

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

/// <summary>
/// Adds the HTTP Bearer authentication scheme and security requirements
/// to the generated OpenAPI document.
/// </summary>
/// <remarks>
/// This allows API consumers and generated clients to understand that
/// authenticated endpoints accept Supabase JWT bearer tokens.
/// </remarks>
internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    /// <summary>
    /// Transforms the generated OpenAPI document by adding the Bearer
    /// authentication scheme and applying it to protected operations.
    /// </summary>
    /// <param name="document">The OpenAPI document being generated.</param>
    /// <param name="context">The OpenAPI document transformation context.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous transformation operation.</returns>
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes =
            await authenticationSchemeProvider.GetAllSchemesAsync();

        if (!authenticationSchemes.Any(
            authScheme => authScheme.Name == JwtBearerDefaults.AuthenticationScheme))
        {
            return;
        }

        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes =
            new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT"
                }
            };

        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                if (path.Key == "/api/v1/health")
                {
                    continue;
                }

                operation.Value.Security ??= [];

                operation.Value.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            }
                        ] = []
                    });
            }
        }
    }
}