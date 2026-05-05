using Asp.Versioning;
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

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    public static WebApplication UseWebApi(this WebApplication app)
    {
        app.UseExceptionHandler();
        
        app.MapDocumentEndpoints();
        return app;
    }
}
