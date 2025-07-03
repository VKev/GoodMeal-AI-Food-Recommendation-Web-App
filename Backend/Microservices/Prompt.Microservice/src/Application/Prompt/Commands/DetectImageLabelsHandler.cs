using Google.Cloud.Vision.V1;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Prompt.Commands;

public sealed record DetectImageLabelsCommand(string ImageUrl) : ICommand<List<string>>;

public class DetectImageLabelsHandler : ICommandHandler<DetectImageLabelsCommand, List<string>>
{
    public async Task<Result<List<string>>> Handle(DetectImageLabelsCommand request,
        CancellationToken cancellationToken)
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "/src/service-account.json");

        var client = await ImageAnnotatorClient.CreateAsync();
        var image = Image.FromUri(request.ImageUrl);

        var labels = await client.DetectLabelsAsync(image);

        var result = labels.Select(l => l.Description.ToLowerInvariant()).ToList();

        return Result.Success(result);
    }
}