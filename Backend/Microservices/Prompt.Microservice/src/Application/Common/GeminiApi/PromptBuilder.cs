namespace Application.Common.GeminiApi
{
    public static class PromptBuilder
    {
        public static string BuildPrompt(string userRequest)
        {
            return $@"Trả lời duy nhất bằng một JSON object có định dạng sau:

          {{
            ""ResponseMessage"": ""string mô tả ngắn về kết quả hoặc 'Yêu cầu nhập lại'"",
            ""Foods"": [
              {{
                ""FoodName"": ""string"",
                ""National"": ""Việt Nam"",
                ""Description"": ""mô tả ngắn về món ăn""
              }}
            ],
            ""Location"": ""string (nếu trong yêu cầu có địa điểm thì điền, nếu không thì null)""
          }}

          Hướng dẫn:
          - Nếu câu yêu cầu dưới đây là một yêu cầu có thể trả về danh sách món ăn Việt Nam (ví dụ: đề xuất món ăn, gợi ý món ăn, liệt kê món ăn...), hãy trả về 10 món ăn Việt Nam kèm mô tả ngắn và địa điểm nếu có.
          - Nếu trong câu yêu cầu có đề cập đến địa điểm nào, lấy đúng tên địa điểm đó và điền vào trường ""Location"". Nếu không có thì ghi ""null"".
          - Nếu câu yêu cầu không phải là một yêu cầu về món ăn, hoặc không thể trả về danh sách món ăn hợp lệ (ví dụ: hỏi về lịch sử, thời tiết, hay thông tin ngoài lĩnh vực ẩm thực), trả về JSON với:
            - ""ResponseMessage"": ""Yêu cầu nhập lại""
            - ""Foods"": [] (mảng rỗng)
            - ""Location"": ""null""
          - Tuyệt đối không được trả bất kỳ văn bản, tiêu đề hay dấu ``` nào khác ngoài JSON object thuần.

          Yêu cầu người dùng:
          ""{userRequest}""";
        }
    }
}