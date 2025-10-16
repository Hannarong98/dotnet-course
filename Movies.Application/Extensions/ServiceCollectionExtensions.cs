using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movies.Application.Auth;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Extensions;

public static class ServiceCollectionExtensions
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
            .AddPolicy(Roles.Write, policy => { policy.RequireRole(Roles.Write); });
        return services;
    }

    public static IServiceCollection AddOpenApiWithSecuritySchemes(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Description = "OAuth2 authentication using JWT bearer tokens.",
                    Scheme = "bearer",
                    Type = SecuritySchemeType.OAuth2,
                    Reference = new OpenApiReference
                    {
                        Id = "OAuth2",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
                document.Components.SecuritySchemes[scheme.Reference.Id] = scheme;
                document.SecurityRequirements ??= [];
                document.SecurityRequirements.Add(new OpenApiSecurityRequirement { [scheme] = [] });

                return Task.CompletedTask;
            });
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
        services.AddSingleton<IDbConnectionFactory>(_ =>
            new NpgsqlConnectionFactory(configuration["Database:ConnectionString"]!));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}