using SharedLibrary.Common.Messaging;

namespace Application.Auths.Commands;

public sealed record LoginWithExternalProviderCommand(
    string IdentityToken
) : ICommand;

