using Application.Abstractions.Messaging;

namespace Application.Auths.Commands;

public sealed record LoginUserCommand(
    string Email,
    string Password
) : ICommand;