namespace Application.Prompt.GeminiApi
{
    public static class PromptBuilder
    {
        public static string BuildPrompt(string userRequest)
        {
            return $@"Trả lời duy nhất bằng JSON object có định dạng sau:
            {{
              ""ResponseMessage"": ""string""
            }}
            Chỉ trả về JSON thuần, KHÔNG kèm dấu ``` hoặc bất kỳ định dạng markdown nào khác.
            Không thêm lời chào, mô tả hay đoạn văn nào khác ngoài JSON.
            KHÔNG trả về file markdown, chỉ JSON thuần.
            Yêu cầu: {userRequest}";
        }
    }
}