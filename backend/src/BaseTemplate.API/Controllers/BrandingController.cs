using Asp.Versioning;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Settings.Commands.UpdateBrandingSettings;
using BaseTemplate.Application.Settings.Dtos;
using BaseTemplate.Application.Settings.Queries.GetBrandingSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

/// <summary>
/// Serves the app's name/logo/colors from the database instead of a build-time
/// constant, so branding can change without a frontend redeploy. The frontend
/// fetches this once at startup — see BrandingService in the Angular app.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/branding")]
[Produces("application/json")]
public sealed class BrandingController(IDispatcher dispatcher) : ControllerBase
{
    /// <summary>Gets the current branding settings. Public — the frontend needs this before any login.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<BrandingSettingsDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BrandingSettingsDto>> Get(CancellationToken cancellationToken)
    {
        var settings = await dispatcher.Send(new GetBrandingSettingsQuery(), cancellationToken);
        return Ok(settings);
    }

    /// <summary>
    /// Updates the branding settings. Requires authentication — since this template's
    /// JWT auth is scaffolding only (see JwtSettings), this endpoint isn't callable
    /// until a real login flow issues tokens.
    /// </summary>
    [HttpPut]
    [Authorize]
    [ProducesResponseType<BrandingSettingsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BrandingSettingsDto>> Update(UpdateBrandingSettingsCommand command, CancellationToken cancellationToken)
    {
        var settings = await dispatcher.Send(command, cancellationToken);
        return Ok(settings);
    }
}
