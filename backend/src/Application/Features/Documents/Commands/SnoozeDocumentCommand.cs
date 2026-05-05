using Application.Abstractions;
using Application.Cqrs;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Features.Documents.Commands;

public sealed record SnoozeDocumentCommand(Guid DocumentId) : ICommand<Document>;

public sealed class SnoozeDocumentCommandHandler(IDocumentRepository repository, IDateTimeProvider dateTimeProvider)
    : ICommandHandler<SnoozeDocumentCommand, Document>
{
    public async Task<Document> HandleAsync(SnoozeDocumentCommand command, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetByIdAsync(command.DocumentId, cancellationToken)
            ?? throw new DocumentNotFoundException(command.DocumentId);

        document.Snooze(dateTimeProvider);
        await repository.UpdateReminderSnoozeAsync(document, cancellationToken);

        return document;
    }
}