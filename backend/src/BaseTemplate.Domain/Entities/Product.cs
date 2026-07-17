using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Events;
using BaseTemplate.Domain.Exceptions;
using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Domain.Entities;

/// <summary>
/// Sample aggregate root demonstrating the entity pattern new features should follow:
/// private setters, a named factory method that enforces invariants and raises a
/// domain event, and intention-revealing behavior methods instead of public setters.
/// </summary>
public sealed class Product : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;

    public string Sku { get; private set; } = null!;

    public string? Description { get; private set; }

    public Money Price { get; private set; } = null!;

    private Product()
    {
        // Required by EF Core.
    }

    public static Product Create(string name, string sku, Money price, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name is required.");
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new DomainException("Product SKU is required.");
        }

        var product = new Product
        {
            Name = name.Trim(),
            Sku = sku.Trim().ToUpperInvariant(),
            Price = price,
            Description = description?.Trim(),
        };

        product.AddDomainEvent(new ProductCreatedDomainEvent(product.Id, product.Name));

        return product;
    }

    public void ChangePrice(Money newPrice)
    {
        if (newPrice == Price)
        {
            return;
        }

        var oldPrice = Price;
        Price = newPrice;
        AddDomainEvent(new ProductPriceChangedDomainEvent(Id, oldPrice.Amount, newPrice.Amount));
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name is required.");
        }

        Name = name.Trim();
    }
}
