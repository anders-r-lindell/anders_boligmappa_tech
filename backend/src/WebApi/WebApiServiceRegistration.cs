using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Authorization;
using WebApi.Features.Documents;

namespace WebApi;

public static class WebApiServiceRegistration
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(ConfigureJwtBearer);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("DocumentOwner", policy => policy.Requirements.Add(new DocumentOwnerRequirement()));
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IAuthorizationHandler, DocumentOwnerAuthorizationHandler>();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    public static WebApplication UseWebApi(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapDocumentEndpoints();
        
        return app;
    }

    // This is just to be used as an example, configuration is skipping important validations as-is
    private static void ConfigureJwtBearer(JwtBearerOptions options)
    {
        options.UseSecurityTokenValidators = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireSignedTokens = false,
            SignatureValidator = (token, _) => new JwtSecurityToken(token)
        };
    }
}
