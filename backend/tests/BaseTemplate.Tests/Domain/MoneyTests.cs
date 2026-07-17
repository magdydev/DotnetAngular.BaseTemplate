using BaseTemplate.Domain.Exceptions;
using BaseTemplate.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BaseTemplate.Tests.Domain;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmountAndCurrency_Succeeds()
    {
        var money = Money.Create(9.99m, "usd");

        money.Amount.Should().Be(9.99m);
        money.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public void Create_WithNegativeAmount_ThrowsDomainException(decimal amount)
    {
        var act = () => Money.Create(amount, "USD");

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("DOLLAR")]
    public void Create_WithInvalidCurrency_ThrowsDomainException(string currency)
    {
        var act = () => Money.Create(10m, currency);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Equality_IsStructural()
    {
        var a = Money.Create(10m, "USD");
        var b = Money.Create(10m, "USD");

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Add_WithDifferentCurrency_ThrowsDomainException()
    {
        var usd = Money.Create(10m, "USD");
        var eur = Money.Create(5m, "EUR");

        var act = () => usd.Add(eur);

        act.Should().Throw<DomainException>();
    }
}
