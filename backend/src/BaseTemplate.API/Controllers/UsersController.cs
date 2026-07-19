using System.Security.Claims;
using Asp.Versioning;
using BaseTemplate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.API.Controllers;

/// <summary>
/// Admin-only CRUD over ASP.NET Core Identity users. Stays in the API layer and
/// talks to UserManager directly rather than going through Application/CQRS —
/// same choice AuthController makes, since Identity already is a persistence
/// abstraction and AppUser lives in Infrastructure (referencing it from
/// Application would invert the dependency direction Clean Architecture relies on).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public sealed class UsersController(
    UserManager<AppUser> userManager,
    ILogger<UsersController> logger) : ControllerBase
{
    public sealed record UserDto(Guid Id, string Username, string? Email, IReadOnlyList<string> Roles, bool LockedOut);

    public sealed record CreateUserRequest(string Username, string? Email, string Password, IReadOnlyList<string>? Roles);

    public sealed record UpdateUserRequest(string? Email, IReadOnlyList<string>? Roles);

    public sealed record ResetPasswordRequest(string NewPassword);

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await userManager.Users.OrderBy(u => u.UserName).ToListAsync(cancellationToken);

        var result = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            result.Add(await ToDto(user));
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        return user is null ? NotFound() : Ok(await ToDto(user));
    }

    [HttpPost]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Create(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(new { message = "Username is required." });
        }

        var user = new AppUser
        {
            UserName = request.Username.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            EmailConfirmed = true,
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(new { message = DescribeErrors(createResult) });
        }

        var roles = request.Roles ?? [];
        if (roles.Count > 0)
        {
            var roleResult = await userManager.AddToRolesAsync(user, roles);
            if (!roleResult.Succeeded)
            {
                logger.LogWarning(
                    "Created user {Username} but failed to assign roles: {Errors}",
                    user.UserName,
                    DescribeErrors(roleResult));
            }
        }

        var dto = await ToDto(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id, version = "1.0" }, dto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Update(Guid id, UpdateUserRequest request)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        if (IsCurrentUser(id) && request.Roles is not null && !request.Roles.Contains("Admin"))
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            if (currentRoles.Contains("Admin"))
            {
                return BadRequest(new { message = "You cannot remove your own Admin role." });
            }
        }

        user.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return BadRequest(new { message = DescribeErrors(updateResult) });
        }

        if (request.Roles is not null)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(request.Roles).ToList();
            var rolesToAdd = request.Roles.Except(currentRoles).ToList();

            if (rolesToRemove.Count > 0)
            {
                await userManager.RemoveFromRolesAsync(user, rolesToRemove);
            }

            if (rolesToAdd.Count > 0)
            {
                await userManager.AddToRolesAsync(user, rolesToAdd);
            }
        }

        return Ok(await ToDto(user));
    }

    [HttpPut("{id:guid}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword(Guid id, ResetPasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = DescribeErrors(result) });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (IsCurrentUser(id))
        {
            return BadRequest(new { message = "You cannot delete your own account." });
        }

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = DescribeErrors(result) });
        }

        return NoContent();
    }

    private bool IsCurrentUser(Guid id) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentId) && currentId == id;

    private async Task<UserDto> ToDto(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var lockedOut = await userManager.IsLockedOutAsync(user);
        return new UserDto(user.Id, user.UserName!, user.Email, roles.ToList(), lockedOut);
    }

    private static string DescribeErrors(IdentityResult result) =>
        string.Join(" ", result.Errors.Select(e => e.Description));
}
