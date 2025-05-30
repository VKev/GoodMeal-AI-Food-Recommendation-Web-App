using Application.Abstractions.Messaging;

namespace Application.Auths.Commands;

public sealed record LoginUserQuery(
    string Email,
    string Password
) : ICommand;