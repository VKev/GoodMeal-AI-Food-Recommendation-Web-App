using System.Text.RegularExpressions;

namespace Application.Common.GeminiApi;

public static class CleanGeminiResponse
{
    public static string CleanStreamResponse(string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
            return rawResponse;

        var startIndex = rawResponse.IndexOf('{');
        var endIndex = rawResponse.LastIndexOf('}');

        if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            throw new InvalidOperationException("Gemini response không chứa JSON hợp lệ.");

        var jsonOnly = rawResponse.Substring(startIndex, endIndex - startIndex + 1);

        return jsonOnly;
    }
    
    public static string CleanResponse(string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
            return rawResponse;
        
        var cleaned = Regex.Replace(rawResponse, @"^```json|```$", string.Empty, RegexOptions.Multiline).Trim();
        
        var startIndex = cleaned.IndexOf('{');
        var endIndex = cleaned.LastIndexOf('}');

        if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
            throw new InvalidOperationException("Gemini response không chứa JSON hợp lệ.");

        var jsonOnly = cleaned.Substring(startIndex, endIndex - startIndex + 1);

        return jsonOnly;
    }
}