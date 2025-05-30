using Application.Abstractions.Messaging;

namespace Application.Auths.Commands;

public sealed record LoginWithExternalProviderCommand(
    string IdentityToken
) : ICommand;