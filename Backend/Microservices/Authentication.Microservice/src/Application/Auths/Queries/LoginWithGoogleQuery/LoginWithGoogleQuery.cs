using Application.Abstractions.Messaging;

namespace Application.Auths.Commands;

public sealed record LoginWithGoogleQuery(
    string IdToken
) : ICommand;