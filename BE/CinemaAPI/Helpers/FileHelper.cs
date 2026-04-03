namespace cinema.Helpers
{
    // FileHelper: cung cấp các phương thức tiện ích liên quan đến xử lý tên file
    public class FileHelper
    {
        /// <summary>
        /// Tạo tên file ngẫu nhiên duy nhất, giữ nguyên phần mở rộng từ tên gốc
        /// </summary>
        /// <param name="fileName">Tên file gốc bao gồm phần mở rộng (ví dụ "image.png")</param>
        /// <returns>Tên file mới dưới dạng GUID không dấu gạch ngang, kèm đuôi mở rộng</returns>
        public static string generateFileName(string fileName)
        {
            // Tạo GUID mới và loại bỏ ký tự '-' để có chuỗi ngắn gọn
            var name = Guid.NewGuid().ToString().Replace("-", "");

            // Xác định vị trí dấu chấm cuối cùng để trích phần mở rộng
            // Ví dụ với "a.doc.gif.jpg.png", lastIndex sẽ là vị trí trước ".png"
            var lastIndex = fileName.LastIndexOf('.');

            // Lấy phần mở rộng, bao gồm dấu chấm (ví dụ ".png")
            var ext = fileName.Substring(lastIndex);

            // Trả về tên file mới = GUID + phần mở rộng
            return name + ext;
        }
    }
}
