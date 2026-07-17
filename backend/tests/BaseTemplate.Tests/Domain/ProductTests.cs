using BaseTemplate.Domain.Events;
using BaseTemplate.Domain.Exceptions;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BaseTemplate.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_RaisesProductCreatedDomainEvent()
    {
        var price = Money.Create(19.99m, "USD");

        var product = Product.Create("Widget", "wgt-1", price, "A sample widget");

        product.Name.Should().Be("Widget");
        product.Sku.Should().Be("WGT-1");
        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductCreatedDomainEvent>();
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsDomainException()
    {
        var price = Money.Create(1m, "USD");

        var act = () => Product.Create(string.Empty, "sku", price);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ChangePrice_ToDifferentValue_RaisesPriceChangedDomainEvent()
    {
        var product = Product.Create("Widget", "WGT-1", Money.Create(10m, "USD"));
        product.ClearDomainEvents();

        product.ChangePrice(Money.Create(15m, "USD"));

        product.Price.Amount.Should().Be(15m);
        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductPriceChangedDomainEvent>();
    }

    [Fact]
    public void ChangePrice_ToSameValue_DoesNotRaiseEvent()
    {
        var product = Product.Create("Widget", "WGT-1", Money.Create(10m, "USD"));
        product.ClearDomainEvents();

        product.ChangePrice(Money.Create(10m, "USD"));

        product.DomainEvents.Should().BeEmpty();
    }
}
