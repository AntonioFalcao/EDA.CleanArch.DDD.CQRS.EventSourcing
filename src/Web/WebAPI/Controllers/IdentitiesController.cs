using Contracts.Services.Identity;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Abstractions;
using WebAPI.ValidationAttributes;

namespace WebAPI.Controllers;

public class IdentitiesController : ApplicationController
{
    public IdentitiesController(IBus bus)
        : base(bus) { }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(Projection.UserAuthentication), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetUserAuthenticationAsync([NotEmpty] Guid userId, CancellationToken cancellationToken)
        => GetProjectionAsync<Query.GetUserAuthentication, Projection.UserAuthentication>(new(userId), cancellationToken);

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> ChangePasswordAsync([NotEmpty] Guid userId, Request.ChangePassword request, CancellationToken cancellationToken)
        => SendCommandAsync<Command.ChangePassword>(new(userId, request.NewPassword, request.NewPasswordConfirmation), cancellationToken);

    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> DeleteAsync([NotEmpty] Guid userId, CancellationToken cancellationToken)
        => SendCommandAsync<Command.Delete>(new(userId), cancellationToken);

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> RegisterAsync(Command.Register command, CancellationToken cancellationToken)
        => SendCommandAsync(command, cancellationToken);
}