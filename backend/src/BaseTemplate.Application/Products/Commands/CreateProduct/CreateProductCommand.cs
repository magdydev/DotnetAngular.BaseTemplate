using BaseTemplate.Application.Common.Interfaces;

namespace BaseTemplate.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Sku,
    decimal Price,
    string Currency,
    string? Description) : ICommand<Guid>;
