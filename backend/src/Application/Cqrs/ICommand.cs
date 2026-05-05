namespace Application.Cqrs;

// Native CQRS abstractions — no MediatR or other NuGet dependency

public interface ICommand<TResult> { }
