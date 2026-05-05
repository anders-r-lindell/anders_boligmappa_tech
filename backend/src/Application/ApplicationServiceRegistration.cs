using Application.Cqrs;
using Application.Features.Documents.Commands;
using Application.Features.Documents.Queries;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        services.AddScoped<IQueryHandler<GetExpiringDocumentsByPropertyIdQuery, IReadOnlyList<Document>>, GetExpiringDocumentsQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllExpiringDocumentsQuery, IReadOnlyList<Document>>, GetAllExpiringDocumentsQueryHandler>();

        services.AddScoped<ICommandHandler<SnoozeDocumentCommand, Document>, SnoozeDocumentCommandHandler>();

        return services;
    }
}
