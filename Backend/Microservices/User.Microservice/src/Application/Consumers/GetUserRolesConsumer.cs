using MassTransit;
using SharedLibrary.Contracts.GetUserRoles;
using Application.Users.Queries;
using MediatR;
using GetUserRolesResponse = SharedLibrary.Contracts.GetUserRoles.GetUserRolesResponse;

namespace Application.Users.Consumers;

public class GetUserRolesConsumer : IConsumer<GetUserRolesRequest>
{
    private readonly IMediator _mediator;

    public GetUserRolesConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<GetUserRolesRequest> context)
    {
        var query = new GetUserRolesQuery(context.Message.IdentityId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            var response = new GetUserRolesResponse
            {
                UserId = result.Value.UserId,
                Email = result.Value.Email,
                Name = result.Value.Name,
                IdentityId = result.Value.IdentityId,
                Roles = result.Value.Roles
            };

            await context.RespondAsync(response);
        }
        else
        {
            throw new Exception($"Failed to get user roles: {result.Error.Description}");
        }
    }
} 