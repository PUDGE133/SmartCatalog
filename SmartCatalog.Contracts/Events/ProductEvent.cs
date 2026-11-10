namespace SmartCatalog.Contracts.Events;

public record ProductCreatedEvent(Guid ProductId);
public record ProductUpdatedEvent(Guid ProductId);
public record ProductDeletedEvent(Guid ProductId);