namespace Domain.Exceptions;

public sealed class DocumentNotFoundException(Guid documentId)
    : DomainException($"Document '{documentId}' was not found.");
