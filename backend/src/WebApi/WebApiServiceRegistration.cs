using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Authorization;
using WebApi.Features.Documents;
using WebApi.Swagger;

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

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(ConfigureSwagger);

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
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            });
        }

        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapDocumentEndpoints();
        
        return app;
    }

    private static void ConfigureSwagger(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Boligmappa API", Version = "v1" });
        options.OperationFilter<RemoveVersionParameterFilter>();
        options.DocumentFilter<ReplaceVersionInPathDocumentFilter>();

        var bearerScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        };

        options.AddSecurityDefinition("Bearer", bearerScheme);
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
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
