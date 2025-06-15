using SharedLibrary.Common.Messaging;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using FirebaseAdmin.Auth;
using MassTransit;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands;

public class LoginWithExternalProviderCommandHandler : ICommandHandler<LoginWithExternalProviderCommand>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IAuthRepository _authRepository;
    private readonly IUserService _userService;

    public LoginWithExternalProviderCommandHandler(
        IAuthRepository authRepository,
        IPublishEndpoint publishEndpoint,
        IUserService userService)
    {
        _authRepository = authRepository;
        _publishEndpoint = publishEndpoint;
        _userService = userService;
    }

    public async Task<Result> Handle(LoginWithExternalProviderCommand request, CancellationToken cancellationToken)
    {
        var jwt = await _authRepository.LoginWithExternalProviderAsync(request.IdentityToken, cancellationToken);

        var userRoles = await _userService.GetUserRolesAsync(jwt.IdentityId.ToString(), cancellationToken);

        var roles = userRoles?.Roles?.ToList() ?? new List<string> { "User" };
        var primaryRole = roles.FirstOrDefault() ?? "User";

        if (userRoles == null)
        {
            throw new ApplicationException("User does not exist");
        }

        var customClaims = new Dictionary<string, object>
        {
            ["email"] = userRoles.Email ?? "",
            ["name"] = userRoles.Name ?? "",
            ["roles"] = userRoles.Roles ?? new List<string>() { "User" },
            ["user_id"] = userRoles.IdentityId,
            ["system_user_id"] = userRoles.UserId.ToString(),
        };

        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(jwt.IdentityId, customClaims);

        var loginResponse = new LoginResponse
        {
            UserId = userRoles.UserId,
            Email = jwt.Email,
            Name = jwt.Name,
            IdToken = request.IdentityToken,
            Roles = roles,
            PrimaryRole = primaryRole,
            ExpiresIn = 3600
        };

        return Result.Success(loginResponse);
    }
}