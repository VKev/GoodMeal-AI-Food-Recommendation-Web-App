using Application.Users.Commands;
using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Utils;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ApiController
    {
        public UserController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        
        [HttpGet("read")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("roles/{identityId}")]
        public async Task<IActionResult> GetUserRoles(string identityId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserRolesQuery(identityId), cancellationToken);
            Console.WriteLine("Get role success: " + result.IsSuccess);

            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            foreach (var role in result.Value.Roles)
            {
                Console.WriteLine(role);
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