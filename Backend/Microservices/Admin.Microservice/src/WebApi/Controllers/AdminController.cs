using Application.Admin.Commands.AddUserRoleCommand;
using Application.Admin.Commands.BusinessCommand;
using Application.Admin.Commands.BusinessCommand.ActiveBusinessCommand;
using Application.Admin.Commands.DeleteUserCommand;
using Application.Admin.Commands.DisableUserCommand;
using Application.Admin.Commands.EnableUserCommand;
using Application.Admin.Commands.RemoveUserRoleCommand;
using Application.Admin.Commands.UpdateUserCommand;
using Application.Admin.Queries.GetUserRolesQuery;
using Application.Admin.Queries.GetUserStatusQuery;
using Application.Admin.Queries.SearchUsersQuery;
using Application.Admin.Queries.GetAllBusinessesQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Utils.AuthenticationExtention;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : ApiController
    {
        public AdminController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("roles/{identityId}")]
        [ApiGatewayUser]
        public async Task<IActionResult> GetUserRoles(string identityId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserRolesQuery(identityId), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("add-role")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> AddRole([FromBody] AddUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("remove-role")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("disable")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> DisableUser([FromBody] DisableUserCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("enable")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> EnableUser([FromBody] EnableUserCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpGet("status/{identityId}")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> GetUserStatus(string identityId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserStatusQuery(identityId), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpGet("users/search")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? searchTerm = null,
            [FromQuery] int pageSize = 50, [FromQuery] string? nextPageToken = null,
            CancellationToken cancellationToken = default)
        {
            var query = new SearchUsersQuery(searchTerm, pageSize, nextPageToken);
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPut("users")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpDelete("users/{identityId}")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string identityId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteUserCommand(identityId), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpGet("businesses")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> GetAllBusinesses(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllBusinessesQuery(), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("businesses/{businessId}/active")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> ActiveBusiness(Guid businessId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ActiveBusinessCommand(businessId), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("businesses/{businessId}/deactive")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> DeactiveBusiness(Guid businessId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeactiveBusinessCommand(businessId), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            return Ok();
        }
    }
}