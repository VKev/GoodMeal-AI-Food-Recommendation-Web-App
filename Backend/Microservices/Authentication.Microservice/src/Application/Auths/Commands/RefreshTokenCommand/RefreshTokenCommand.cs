using Application.Abstractions.Messaging;

namespace Application.Auths.Commands;

public sealed record RefreshTokenCommand(
    string RefreshToken
) : ICommand;