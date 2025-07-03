namespace Application.Common.GeminiApi
{
    public static class PromptBuilder
    {
        public static string BuildPrompt(string userRequest)
        {
            return $@"Chỉ trả về một JSON object đúng định dạng:

                {{
                  ""Error"": ""null hoặc 'Yêu cầu nhập lại'"",
                  ""Title"": ""mô tả ngắn"",
                  ""Foods"": [
                    {{
                      ""FoodName"": ""string"",
                      ""National"": ""nơi xuất xứ hoặc 'Việt Nam'"",
                      ""Description"": ""mô tả ngắn"",
                      ""ImageUrl"": null
                    }}
                  ],
                  ""Location"": ""string hoặc 'Việt Nam'""
                }}
            Quy tắc:
            - Nếu yêu cầu là về món ăn Việt Nam → trả về 10 món ăn cụ thể đã chế biến (ví dụ: ""Phở bò"", ""Tôm hấp bia"") và Location nếu có.
            - Nếu không phải yêu cầu về món ăn hoặc không hợp lệ → Error: 'Yêu cầu nhập lại', các trường khác để trống hoặc null.
            - Không thêm văn bản ngoài JSON.

            Yêu cầu: ""{userRequest}""
            ";
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