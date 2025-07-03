namespace Application.Prompt.Commands;

public class ImageSearchResponse
{
    public List<ImageResult> Images { get; set; } = new();
}

public record ImageResult
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int ThumbnailWidth { get; set; }
    public int ThumbnailHeight { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string GoogleUrl { get; set; } = string.Empty;
}