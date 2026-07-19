using Asp.Versioning;
using BaseTemplate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.API.Controllers;

/// <summary>
/// Admin-only CRUD over ASP.NET Core Identity roles — roles are data, not a
/// hardcoded enum, so new ones can be created (and assigned to users via
/// UsersController) without a code change or redeploy.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/roles")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public sealed class RolesController(
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<AppUser> userManager) : ControllerBase
{
    private const string ProtectedRoleName = "Admin";

    public sealed record RoleDto(Guid Id, string Name, int UserCount, bool Protected);

    public sealed record CreateRoleRequest(string Name);

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<RoleDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.OrderBy(r => r.Name).ToListAsync(cancellationToken);

        var result = new List<RoleDto>(roles.Count);
        foreach (var role in roles)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name!);
            result.Add(new RoleDto(role.Id, role.Name!, usersInRole.Count, IsProtected(role.Name)));
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType<RoleDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> Create(CreateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Role name is required." });
        }

        var name = request.Name.Trim();
        if (await roleManager.RoleExistsAsync(name))
        {
            return BadRequest(new { message = $"Role '{name}' already exists." });
        }

        var role = new IdentityRole<Guid>(name);
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });
        }

        return CreatedAtAction(nameof(GetAll), new { version = "1.0" }, new RoleDto(role.Id, role.Name!, 0, false));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());
        if (role is null)
        {
            return NotFound();
        }

        if (IsProtected(role.Name))
        {
            return BadRequest(new { message = $"The '{ProtectedRoleName}' role cannot be deleted." });
        }

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });
        }

        return NoContent();
    }

    private static bool IsProtected(string? roleName) =>
        string.Equals(roleName, ProtectedRoleName, StringComparison.OrdinalIgnoreCase);
}
