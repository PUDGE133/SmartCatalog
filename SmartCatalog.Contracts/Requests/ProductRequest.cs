namespace SmartCatalog.Contracts.Requests;

public record ProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    uint Version);