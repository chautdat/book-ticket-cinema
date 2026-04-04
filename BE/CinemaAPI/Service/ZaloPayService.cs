using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using cinema.Helpers;
using cinema.Models;
using Microsoft.Extensions.Options;
using System.Linq;

namespace cinema.Services
{
    /// <summary>
    /// Tích hợp ZaloPay (sandbox). Chỉ tạo order và xử lý callback, không hardcode secret.
    /// </summary>
    public class ZaloPayService
    {
        private readonly HttpClient _httpClient;
        private readonly ZaloPayOptions _options;
        private readonly PaymentService _paymentService;
        private readonly BookingService _bookingService;
        private readonly ILogger<ZaloPayService> _logger;

        public ZaloPayService(HttpClient httpClient,
                              IOptions<ZaloPayOptions> options,
                              PaymentService paymentService,
                              BookingService bookingService,
                              ILogger<ZaloPayService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _paymentService = paymentService;
            _bookingService = bookingService;
            _logger = logger;
        }

        public class CreateOrderResult
        {
            public string OrderUrl { get; set; } = string.Empty;
            public string TransToken { get; set; } = string.Empty;
            public string AppTransId { get; set; } = string.Empty;
            public int PaymentId { get; set; }
        }

        public class QueryOrderResult
        {
            public string AppTransId { get; set; } = string.Empty;
            public int ReturnCode { get; set; }
            public string ReturnMessage { get; set; } = string.Empty;
            public int SubReturnCode { get; set; }
            public string SubReturnMessage { get; set; } = string.Empty;
            public bool IsProcessing { get; set; }
            public long ZpTransId { get; set; }
            public bool IsSuccess { get; set; }
            public bool IsPending { get; set; }
        }

        public async Task<CreateOrderResult> CreateOrderAsync(int bookingId, double amount, string appUser, string returnUrl)
        {
            var appTransId = $"{DateTime.Now:yyMMdd}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
            var appTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // embed_data phải là JSON object với các trường ZaloPay yêu cầu
            var embedData = JsonSerializer.Serialize(new { redirecturl = returnUrl, bookingId = bookingId });
            var itemJson = "[]"; // Empty array

            // Format đúng theo ZaloPay docs: app_id|app_trans_id|app_user|amount|app_time|embed_data|item
            var dataMac = $"{_options.AppId}|{appTransId}|{appUser}|{(long)amount}|{appTime}|{embedData}|{itemJson}";
            var mac = HmacSha256(_options.Key1, dataMac);
            
            _logger.LogInformation("ZaloPay MAC data: {dataMac}", dataMac);

            var payload = new Dictionary<string, string>
            {
                ["app_id"] = _options.AppId.ToString(),
                ["app_user"] = appUser,
                ["app_time"] = appTime.ToString(),
                ["amount"] = ((long)amount).ToString(),
                ["app_trans_id"] = appTransId,
                ["embed_data"] = embedData,
                ["item"] = itemJson,
                ["description"] = $"CGV Cinema - Thanh toan ve xem phim",
                ["mac"] = mac,
                ["bank_code"] = ""
            };
            if (!string.IsNullOrWhiteSpace(_options.CallbackUrl))
            {
                payload["callback_url"] = _options.CallbackUrl;
            }

            var content = new FormUrlEncodedContent(payload);
            var response = await _httpClient.PostAsync(_options.CreateOrderUrl, content);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("ZaloPay create order response: {body}", body);

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var returnCode = root.GetProperty("return_code").GetInt32();
            if (returnCode != 1)
            {
                var message = root.GetProperty("return_message").GetString();
                throw new InvalidOperationException($"Create ZaloPay order failed: {message}");
            }

            // KHÔNG lưu payment ở đây nữa - chỉ trả về orderUrl
            // Payment sẽ được tạo khi thanh toán thành công (từ frontend)

            // ZaloPay trả về cả order_url (mở in-app) và cashier_order_url (trang web thu ngân).
            // Trên web nên ưu tiên cashier_order_url để tránh trang trắng ở qcgateway.
            var orderUrl = root.TryGetProperty("cashier_order_url", out var cashierUrl)
                ? cashierUrl.GetString()
                : null;
            orderUrl ??= root.GetProperty("order_url").GetString() ?? string.Empty;

            return new CreateOrderResult
            {
                OrderUrl = orderUrl,
                TransToken = root.GetProperty("zp_trans_token").GetString() ?? string.Empty,
                AppTransId = appTransId,
                PaymentId = 0 // Chưa có payment
            };
        }

        public async Task<bool> HandleCallbackAsync(Dictionary<string, string> form)
        {
            // ZaloPay gửi data + mac, cần verify với key2
            if (!form.TryGetValue("data", out var data) || !form.TryGetValue("mac", out var reqMac))
            {
                _logger.LogWarning("Callback missing data or mac");
                return false;
            }

            var mac = HmacSha256(_options.Key2, data);
            if (mac != reqMac)
            {
                _logger.LogWarning("Callback mac invalid");
                return false;
            }

            var dataObj = JsonSerializer.Deserialize<ZaloCallbackData>(data);
            if (dataObj == null)
            {
                _logger.LogWarning("Callback data invalid json");
                return false;
            }

            // Lấy bookingId từ embed_data
            var embed = JsonSerializer.Deserialize<EmbedData>(dataObj.embed_data ?? "{}");
            var bookingId = embed?.bookingId ?? 0;

            if (dataObj.return_code == 1)
            {
                // Thanh toán thành công
                // Tìm payment theo TransactionNo (app_trans_id) và update
                var payment = FindPaymentByTransaction(dataObj.app_trans_id);
                if (payment != null)
                {
                    _paymentService.updateStatus(payment.Id, true, dataObj.app_trans_id, 2);
                    if (bookingId > 0)
                    {
                        _bookingService.updateStatus(bookingId, true);
                    }
                }
                return true;
            }
            else
            {
                // Thất bại
                var payment = FindPaymentByTransaction(dataObj.app_trans_id);
                if (payment != null)
                {
                    _paymentService.updateStatus(payment.Id, false, dataObj.app_trans_id, 2);
                }
                return false;
            }
        }

        public async Task<QueryOrderResult> QueryOrderAsync(string appTransId)
        {
            if (string.IsNullOrWhiteSpace(appTransId))
            {
                throw new ArgumentException("Thiếu app_trans_id để query ZaloPay");
            }

            // Theo đặc tả ZaloPay query:
            // data = app_id|app_trans_id|key1
            // mac = HMAC_SHA256(key1, data)
            var dataMac = $"{_options.AppId}|{appTransId}|{_options.Key1}";
            var mac = HmacSha256(_options.Key1, dataMac);

            var payload = new Dictionary<string, string>
            {
                ["app_id"] = _options.AppId.ToString(),
                ["app_trans_id"] = appTransId,
                ["mac"] = mac
            };

            var content = new FormUrlEncodedContent(payload);
            var response = await _httpClient.PostAsync(_options.QueryUrl, content);
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("ZaloPay query response ({appTransId}): {body}", appTransId, body);

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            var returnCode = root.TryGetProperty("return_code", out var rcEl) ? rcEl.GetInt32() : -999;
            var returnMessage = root.TryGetProperty("return_message", out var rmEl)
                ? rmEl.GetString() ?? string.Empty
                : string.Empty;
            var subReturnCode = root.TryGetProperty("sub_return_code", out var srcEl) ? srcEl.GetInt32() : 0;
            var subReturnMessage = root.TryGetProperty("sub_return_message", out var srmEl)
                ? srmEl.GetString() ?? string.Empty
                : string.Empty;

            var isProcessing = false;
            if (root.TryGetProperty("is_processing", out var procEl))
            {
                if (procEl.ValueKind == JsonValueKind.True)
                {
                    isProcessing = true;
                }
                else if (procEl.ValueKind == JsonValueKind.Number)
                {
                    isProcessing = procEl.GetInt32() == 1;
                }
                else if (procEl.ValueKind == JsonValueKind.String)
                {
                    var procText = procEl.GetString();
                    isProcessing = string.Equals(procText, "true", StringComparison.OrdinalIgnoreCase) ||
                                   string.Equals(procText, "1", StringComparison.OrdinalIgnoreCase);
                }
            }

            var zpTransId = 0L;
            if (root.TryGetProperty("zp_trans_id", out var ztEl))
            {
                if (ztEl.ValueKind == JsonValueKind.Number)
                {
                    zpTransId = ztEl.GetInt64();
                }
                else if (ztEl.ValueKind == JsonValueKind.String &&
                         long.TryParse(ztEl.GetString(), out var zpTransParsed))
                {
                    zpTransId = zpTransParsed;
                }
            }

            // ZaloPay có thể trả message khác nhau theo môi trường/sandbox.
            // Chuẩn hóa điều kiện để FE nhận trạng thái ổn định hơn.
            var normalizedReturnMessage = returnMessage.Trim().ToLowerInvariant();
            var normalizedSubReturnMessage = subReturnMessage.Trim().ToLowerInvariant();

            var looksLikeSuccess =
                subReturnCode == 1 ||
                normalizedSubReturnMessage.Contains("success") ||
                normalizedReturnMessage.Contains("success");

            var looksLikePending =
                isProcessing ||
                subReturnCode == 2 ||
                subReturnCode == 3 ||
                normalizedSubReturnMessage.Contains("processing") ||
                normalizedSubReturnMessage.Contains("pending");

            var isSuccess = returnCode == 1 && (zpTransId > 0 || looksLikeSuccess);
            var isPending = !isSuccess && (looksLikePending || (returnCode == 3 && zpTransId <= 0));

            return new QueryOrderResult
            {
                AppTransId = appTransId,
                ReturnCode = returnCode,
                ReturnMessage = returnMessage,
                SubReturnCode = subReturnCode,
                SubReturnMessage = subReturnMessage,
                IsProcessing = isProcessing,
                ZpTransId = zpTransId,
                IsSuccess = isSuccess,
                IsPending = isPending
            };
        }

        private Payment? FindPaymentByTransaction(string transNo)
        {
            return _paymentService.findByTransactionNo(transNo);
        }

        private static string HmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private class ZaloCallbackData
        {
            public int app_id { get; set; }
            public string app_trans_id { get; set; } = string.Empty;
            public long app_time { get; set; }
            public string embed_data { get; set; } = string.Empty;
            public string item { get; set; } = string.Empty;
            public long zp_trans_id { get; set; }
            public int return_code { get; set; }
            public string return_message { get; set; } = string.Empty;
        }

        private class EmbedData
        {
            public int bookingId { get; set; }
            public string? redirecturl { get; set; }
        }
    }
}
