namespace cinema.Helpers
{
    /// <summary>
    /// SMSHelper: hỗ trợ gửi tin nhắn SMS.
    /// Bạn có thể triển khai thực tế với Twilio, VNPay SMS, hoặc bất kỳ nhà cung cấp nào.
    /// </summary>
    public class SMSHelper
    {
        public SMSHelper()
        {
            // Constructor: nếu cần khởi tạo cấu hình (đọc appsettings.json, API key,...)
        }

        /// <summary>
        /// Gửi một tin nhắn SMS với nội dung body.
        /// Trả về true nếu gửi thành công, false nếu thất bại.
        /// </summary>
        /// <param name="body">Nội dung tin nhắn</param>
        public bool sendSMS(string body)
        {
            try
            {
                // TODO: Thực hiện gọi API gửi SMS ở đây.
                // Ví dụ minh họa: in ra console để kiểm tra nội dung.
                Console.WriteLine($"[SMSHelper] Gửi SMS với nội dung: {body}");

                // Nếu sử dụng Twilio hoặc dịch vụ khác, thay logic này bằng các API call.
                return true; // Giả sử gửi thành công
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để debug
                Console.WriteLine($"[SMSHelper] Lỗi khi gửi SMS: {ex.Message}");
                return false; // Trả về thất bại nếu exception
            }
        }
    }
}
