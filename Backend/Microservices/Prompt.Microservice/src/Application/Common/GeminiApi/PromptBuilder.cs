namespace Application.Common.GeminiApi
{
    public static class PromptBuilder
    {
        public static string BuildPrompt(string userRequest)
        {
            return $@"Trả lời duy nhất bằng một JSON object có định dạng sau:

        {{
          ""Error"":""null hoặc 'Yêu cầu nhập lại'""
          ""Title"": ""string mô tả ngắn về kết quả "",
          ""Foods"": [
            {{
              ""FoodName"": ""string"",
              ""National"": ""Việt Nam"",
              ""Description"": ""mô tả ngắn về món ăn"",
              ""ImageUrl"": null
            }}
          ],
          ""FoodNames"": [""string"", ""string"", ...],
          ""Location"": ""string (nếu trong yêu cầu có địa điểm thì điền, nếu không thì null)""
        }}

        Hướng dẫn:
        - Nếu câu yêu cầu dưới đây là một yêu cầu có thể trả về danh sách món ăn Việt Nam (ví dụ: đề xuất món ăn, gợi ý món ăn, liệt kê món ăn...), hãy trả về 10 món ăn Việt Nam kèm mô tả ngắn và địa điểm nếu có.
        - Nếu trong câu yêu cầu có đề cập đến địa điểm nào, lấy đúng tên địa điểm đó và điền vào trường ""Location"". Nếu không có thì ghi ""null"".
        - Nếu câu yêu cầu không phải là một yêu cầu về món ăn, hoặc không thể trả về danh sách món ăn hợp lệ (ví dụ: hỏi về lịch sử, thời tiết, hay thông tin ngoài lĩnh vực ẩm thực), trả về JSON với:
          - ""Error"":""'Yêu cầu nhập lại'""
          - ""Title"": ""null""
          - ""Foods"": [] (mảng rỗng)
          - ""FoodNames"": [] (mảng rỗng)
          - ""Location"": ""null""
        - Tuyệt đối không được trả bất kỳ văn bản, tiêu đề hay dấu ``` nào khác ngoài JSON object thuần.

        Yêu cầu người dùng:
        ""{userRequest}""";
        }

        /// <summary>
        /// Tạo prompt yêu cầu AI đánh giá số sao dựa vào lời comment của người dùng
        /// </summary>
        /// <param name="userComment">Nội dung comment người dùng</param>
        /// <returns>Prompt chuẩn để gửi Gemini</returns>
        public static string BuildStarRatingPrompt(string userComment)
        {
            return $@"Bạn là một chuyên gia đánh giá bình luận khách hàng.
                Dựa vào nội dung comment của người dùng về món ăn, hãy đánh giá mức độ hài lòng bằng số sao từ 1.0 đến 5.0 (kiểu float).

                Quy tắc:
                - 5.0: Rất hài lòng, khen ngợi tuyệt đối.
                - 4.0 - 4.9: Hài lòng nhưng có phàn nàn nhẹ.
                - 3.0 - 3.9: Bình thường, khen và chê cân bằng.
                - 2.0 - 2.9: Không hài lòng, phần lớn là chê.
                - 1.0 - 1.9: Rất tệ, toàn lời phàn nàn.

                Yêu cầu phản hồi dưới dạng JSON object, không thêm bất kỳ nội dung nào khác:
                Ví dụ:
                {{
                  ""Stars"": 4.5
                }}
                Nội dung comment:
                ""{userComment}""";
        }
    }
}