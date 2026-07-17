using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Exceptions;

namespace BaseTemplate.Domain.ValueObjects;

/// <summary>
/// Sample value object: an amount paired with an ISO 4217 currency code.
/// Immutable and self-validating — invalid money can never be constructed.
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }

    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new DomainException("Money amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
        {
            throw new DomainException("Currency must be a 3-letter ISO 4217 code.");
        }

        return new Money(amount, currency.ToUpperInvariant());
    }

    public Money Add(Money other)
    {
        if (other.Currency != Currency)
        {
            throw new DomainException($"Cannot add {other.Currency} to {Currency}.");
        }

        return Create(Amount + other.Amount, Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
