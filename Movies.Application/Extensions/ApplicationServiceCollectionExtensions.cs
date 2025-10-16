using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Movies.Application.Auth;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{

    public static IServiceCollection AddAuth(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.Authority = "http://localhost:8080/realms/movies";
            options.Audience = configuration["Jwt:ClientId"];
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                RoleClaimType = ClaimTypes.Role
            };
            options.MapInboundClaims = false;
        });

        services.AddScoped<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

        services.AddAuthorizationBuilder()
            .AddPolicy(Roles.Write, policy =>
            {
                policy.RequireRole(Roles.Write);
            });
        return services;
    }
    
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddSingleton<IRatingRepository, RatingRepository>();
        services.AddSingleton<IRatingService, RatingService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(configuration["Database:ConnectionString"]!));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}