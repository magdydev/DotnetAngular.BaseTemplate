using Asp.Versioning;
using BaseTemplate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/health")]
[Produces("application/json")]
public sealed class HealthController(ApplicationDbContext dbContext, ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = new HealthResponse
        {
            Timestamp = DateTime.UtcNow,
            Status = "healthy",
        };

        try
        {
            _ = await dbContext.BrandingSettings.FirstOrDefaultAsync(cancellationToken);
            result.Database = "connected";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check failed — database unreachable");
            result.Status = "unhealthy";
            result.Database = "disconnected";
            result.Error = ex.Message;
        }

        return result.Status == "healthy"
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }
}

internal sealed class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Database { get; set; } = string.Empty;
    public string? Error { get; set; }
}
