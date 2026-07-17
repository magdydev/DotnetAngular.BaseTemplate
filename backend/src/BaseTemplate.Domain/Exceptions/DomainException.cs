namespace BaseTemplate.Domain.Exceptions;

/// <summary>
/// Raised when a domain invariant is violated. Distinct from application-level
/// validation errors — this represents a business rule broken inside the model itself.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
