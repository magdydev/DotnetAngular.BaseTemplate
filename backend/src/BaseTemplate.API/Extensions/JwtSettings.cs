namespace BaseTemplate.API.Extensions;

/// <summary>
/// Bound from the "Jwt" configuration section. The API is wired to validate
/// bearer tokens shaped like this, but nothing issues tokens yet — plug in a
/// login/token endpoint (or an external identity provider) when auth is needed.
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    /// <summary>Development-only placeholder. In real environments, load this from
    /// user-secrets, an environment variable, or a secret manager — never commit it.</summary>
    public string SigningKey { get; init; } = string.Empty;

    public int ExpiryMinutes { get; init; } = 60;
}
