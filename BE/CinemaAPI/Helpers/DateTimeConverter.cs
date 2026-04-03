using System; // Cho kiểu DateTime
using System.Globalization; // Cho CultureInfo và parsing theo vùng
using System.Text.Json; // Cho Utf8JsonReader và Utf8JsonWriter
using System.Text.Json.Serialization; // Cho JsonConverter<T>

namespace DemoSession1_WebAPI.Converters
{
    // Bộ chuyển đổi DateTime để customize định dạng JSON
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string formatDate = "dd/MM/yyyy HH:mm:ss"; // Định dạng ngày giờ mong muốn

        // Đọc DateTime từ JSON string với định dạng định sẵn
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Lấy chuỗi ngày giờ từ reader
            var str = reader.GetString();
            
            if (string.IsNullOrEmpty(str))
                return default(DateTime);
            
            // Danh sách các format hỗ trợ
            string[] supportedFormats = {
                "dd/MM/yyyy HH:mm:ss",          // Format cũ
                "dd/MM/yyyy",                    // Format ngắn
                "yyyy-MM-dd",                    // ISO format (từ frontend)
                "yyyy-MM-ddTHH:mm:ss",           // ISO với time
                "yyyy-MM-ddTHH:mm:ss.fff",       // ISO với milliseconds
                "yyyy-MM-ddTHH:mm:ss.fffZ",      // ISO với milliseconds + UTC
                "yyyy-MM-ddTHH:mm:ssZ",          // ISO với UTC
                "yyyy-MM-dd HH:mm:ss"            // Alternative format
            };
            
            // Thử parse với từng format
            if (DateTime.TryParseExact(str, supportedFormats, CultureInfo.InvariantCulture, 
                DateTimeStyles.None, out var result))
            {
                return result;
            }
            
            // Fallback: thử parse tự động
            if (DateTime.TryParse(str, out var fallbackResult))
            {
                return fallbackResult;
            }
            
            throw new JsonException($"Cannot convert '{str}' to DateTime. Supported formats: {string.Join(", ", supportedFormats)}");
        }

        // Viết DateTime ra JSON string với định dạng định sẵn
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Chuyển đối tượng DateTime thành chuỗi theo format và ghi vào JSON
            writer.WriteStringValue(value.ToString(formatDate));
        }
    }
}
