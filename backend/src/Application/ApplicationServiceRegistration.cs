using Application.Cqrs;
using Application.Features.Documents.Queries;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;

namespace Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped<IQueryHandler<GetExpiringDocumentsByPropertyIdQuery, IReadOnlyList<Document>>, GetExpiringDocumentsQueryHandler>();

        return services;
    }
}
