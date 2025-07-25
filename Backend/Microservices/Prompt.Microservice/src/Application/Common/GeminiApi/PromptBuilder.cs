namespace Application.Common.GeminiApi
{
    public static class PromptBuilder
    {
        public static string BuildPrompt(string userRequest)
        {
            return $@"
            Chỉ trả về một JSON object đúng định dạng:
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
            - Nếu nội dung yêu cầu **có liên quan đến món ăn/ẩm thực** (ví dụ: tên món, gợi ý món, công thức, ẩm thực vùng miền, v.v.)  
              → Trả về **bắt buộc 9** món ăn cụ thể, phù hợp ngữ cảnh.  
              ● Mỗi phần tử trong ""Foods"" phải điền đủ FoodName, National và Description (ImageUrl để null hoặc điền URL nếu có).  
              ● Nếu người dùng nêu địa danh/xứ sở, điền vào ""Location"" và National tương ứng; nếu không, mặc định ""Việt Nam"".  
            - Chỉ khi nội dung **hoàn toàn không liên quan** đến món ăn/ẩm thực hoặc không thể xác định được ý định  
              → đặt ""Error"": ""Yêu cầu nhập lại"" và để trống/null các trường còn lại.  
            - **Không** thêm bất kỳ văn bản nào ngoài JSON.

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
            return $@"
            Bạn là chuyên gia phân tích cảm xúc khách hàng trong lĩnh vực ẩm thực.
            Nhiệm vụ: đọc **toàn bộ** nội dung bình luận và chấm điểm mức độ hài lòng về món ăn
            bằng số sao **float** trong thang **1.0 – 5.0**, làm tròn 1 chữ số thập phân.

            ## Hướng dẫn đánh giá
            1. **Phân loại cảm xúc tổng quát**: rất hài lòng / hài lòng / trung lập / không hài lòng / rất tệ.  
            2. **Trọng số chi tiết** (áp dụng cho cả lời khen lẫn chê):  
               * Hương vị – 60%  
               * Trình bày – 15%  
               * Giá trị (so với giá tiền) – 15%  
               * Dịch vụ liên quan tới món (tốc độ, thái độ) – 10%  
            3. **Tính “sao thô”** (khởi điểm 3.0★)  
               * Rất khen +2.0, khá khen+1.0, nhẹ khen+0.5  
               * Rất chê −2.0, khá chê−1.0, nhẹ chê−0.5  
            4. **Điều chỉnh ngữ cảnh** (có thể cộng/trừ thêm 0–0.5★):  
               * Nếu bình luận khẳng định “sẽ quay lại” hoặc “rất đáng thử”,+0.2★  
               * Nếu bình luận “không bao giờ quay lại” hoặc “tệ chưa từng”,−0.2★  
            5. **Giới hạn** kết quả trong [1.0 – 5.0] và làm tròn 0.1.

            ## Thang tham chiếu cuối
            * **5.0** ★  Rất hài lòng, khen tuyệt đối, không phàn nàn.  
            * **4.0 – 4.9** ★ Hài lòng, có 1–2 góp ý nhẹ.  
            * **3.0 – 3.9** ★ Trung lập hoặc khen/chê cân bằng.  
            * **2.0 – 2.9** ★ Phần lớn không hài lòng, vẫn còn điểm tích cực.  
            * **1.0 – 1.9** ★ Rất tệ, chê toàn diện.  

            ### Định dạng phản hồi duy nhất
            ```json
            {{
              ""Stars"": <float>
            }}
                Nội dung comment:
                ""{userComment}""";
        }
    }
}