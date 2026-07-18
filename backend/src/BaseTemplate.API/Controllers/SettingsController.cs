using Asp.Versioning;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Settings.Commands.UpdateBrandingSettings;
using BaseTemplate.Application.Settings.Dtos;
using BaseTemplate.Application.Settings.Queries.GetBrandingSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/settings")]
[Produces("application/json")]
public sealed class SettingsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet("branding")]
    [AllowAnonymous]
    [ProducesResponseType<BrandingSettingsDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BrandingSettingsDto>> GetBranding(CancellationToken cancellationToken)
    {
        var settings = await dispatcher.Send(new GetBrandingSettingsQuery(), cancellationToken);
        return Ok(settings);
    }

    [HttpPut("branding")]
    [Authorize]
    [ProducesResponseType<BrandingSettingsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BrandingSettingsDto>> UpdateBranding(UpdateBrandingSettingsCommand command, CancellationToken cancellationToken)
    {
        var settings = await dispatcher.Send(command, cancellationToken);
        return Ok(settings);
    }
}
