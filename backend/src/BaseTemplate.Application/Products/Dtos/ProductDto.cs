namespace BaseTemplate.Application.Products.Dtos;

public sealed class ProductDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Sku { get; init; } = string.Empty;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public string Currency { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
}
