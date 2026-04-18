using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MobyLabWebProgramming.Infrastructure.Configurations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MobyLabWebProgramming.Infrastructure.Converters;
using MobyLabWebProgramming.Infrastructure.Middlewares;
using Serilog;
using Serilog.Events;

namespace MobyLabWebProgramming.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// This extension method adds the CORS configuration to the application builder.
    /// </summary>
    public static WebApplicationBuilder AddCorsConfiguration(this WebApplicationBuilder builder)
    {
        var corsConfiguration = builder.Configuration.GetRequiredSection(nameof(CorsConfiguration)).Get<CorsConfiguration>();

        if (corsConfiguration == null)
        {
            throw new ApplicationException("The CORS configuration needs to be set!");
        }
        
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policyBuilder =>
                {
                    policyBuilder.WithOrigins(corsConfiguration.Origins) // This adds the valid origins that the browser client can have.
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return builder;
    }

    /// <summary>
    /// This extension method adds the advanced logging configuration to the application builder.
    /// </summary>
    public static WebApplicationBuilder UseLogger(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, logger) =>
        {
            logger
                .MinimumLevel.Is(LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .WriteTo.Console();
        });

        return builder;
    }

    /// <summary>
    /// This extension method adds the controllers and JSON serialization configuration to the application builder.
    /// </summary>
    public static WebApplicationBuilder AddApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options => { options.Conventions.Add(new MultipartFormConvention()); });
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
        });
        builder.Services.AddEndpointsApiExplorer()
            .AddMvc()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // Adds a conversion by name of the enums, otherwise numbers representing the enum values are used.
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // This converts the public property names of the objects serialized to Camel case.
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // When deserializing request the properties of the JSON are mapped ignoring the casing.
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict; // Only numbers are mapped to numeric field, not strings.
            });

        return builder;
    }

    /// <summary>
    /// This extension method adds just the authorization configuration to the application builder.
    /// </summary>
    private static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // This is to use the JWT token with the "Bearer" scheme
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtConfiguration = builder.Configuration.GetSection(nameof(JwtConfiguration)).Get<JwtConfiguration>(); // Here we use the JWT configuration from the application.json.

                if (jwtConfiguration == null)
                {
                    throw new ApplicationException("The JWT configuration needs to be set!");
                }
                
                var key = Encoding.ASCII.GetBytes(jwtConfiguration.Key); // Use configured key to verify the JWT signature.
                options.TokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true, // Validate the issuer claim in the JWT. 
                    ValidateAudience = true, // Validate the audience claim in the JWT.
                    ValidAudience = jwtConfiguration.Audience, // Sets the intended audience.
                    ValidIssuer = jwtConfiguration.Issuer, // Sets the issuing authority.
                    ClockSkew = TimeSpan.Zero // No clock skew is added, when the token expires it will immediately become unusable.
                };
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;
            }).Services
            .AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder().AddDefaultPolicy().Build(); // Adds the default policy for the JWT claims.
            });

        return builder;
    }

    /// <summary>
    /// This extension method adds the authorization with the Swagger configuration to the application builder.
    /// </summary>
    public static WebApplicationBuilder AddAuthorizationWithSwagger(this WebApplicationBuilder builder, string application)
    {
        var securityScheme = new OpenApiSecurityScheme // This is to configure the authorization in the Swagger client so that you may test authorized routes.
        {
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new()
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SupportNonNullableReferenceTypes();
            c.UseAllOfToExtendReferenceSchemas();
            c.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
            c.SwaggerDoc("v1", new() { Title = application, Version = "v1" }); // Adds the application name and version, there can be more than one version for the API.
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new()
            {
                {
                    securityScheme,
                    Array.Empty<string>()
                }
            });
        });

        return builder.ConfigureAuthentication();
    }

    /// <summary>
    /// This extension method adds the default authorization policy to the AuthorizationPolicyBuilder.
    /// It requires that the JWT needs to have the given claims in the configuration.
    /// </summary>
    private static AuthorizationPolicyBuilder AddDefaultPolicy(this AuthorizationPolicyBuilder policy) =>
        policy.RequireClaim(ClaimTypes.NameIdentifier)
            .RequireClaim(ClaimTypes.Name)
            .RequireClaim(ClaimTypes.Email);
}