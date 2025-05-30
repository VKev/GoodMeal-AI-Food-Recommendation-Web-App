using Application.Abstractions.Messaging;

namespace Application.Auths.Commands;

public sealed record LoginWithGitHubQuery(
    string AccessToken
) : ICommand;