using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using AutoMapper;
using Domain.Repositories;

namespace Application.Users.Queries
{
    public sealed record GetUserRolesQuery(string IdentityId) : IQuery<GetUserRolesResponse>;

    internal sealed class GetUserRolesQueryHandler : IQueryHandler<GetUserRolesQuery, GetUserRolesResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserRolesQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<GetUserRolesResponse>> Handle(GetUserRolesQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdentityIdAsync(request.IdentityId, cancellationToken);

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

            var response = new GetUserRolesResponse(
                user.UserId,
                user.Email,
                user.Name,
                user.IdentityId ?? "",
                roles
            );

            return Result.Success(response);
        }
    }
}