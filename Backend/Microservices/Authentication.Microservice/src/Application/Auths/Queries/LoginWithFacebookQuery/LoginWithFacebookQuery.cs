using Application.Abstractions.Messaging;

namespace Application.Auths.Commands;

public sealed record LoginWithFacebookQuery(
    string AccessToken
) : ICommand;