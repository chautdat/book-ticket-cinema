using System.Collections.Concurrent; // Cho ConcurrentBag
using System.Linq; // Cho Except
using System.Net.WebSockets; // Cho WebSocket

/// <summary>
/// Quản lý danh sách kết nối WebSocket (client) theo kiểu singleton tĩnh.
/// Cho phép thêm, xoá và truy xuất toàn bộ client đang kết nối.
/// </summary>
public static class WebSocketManager
{
    // Sử dụng ConcurrentBag để lưu các WebSocket client an toàn trong môi trường đa luồng
    private static ConcurrentBag<WebSocket> _webSockets = new ConcurrentBag<WebSocket>();

    /// <summary>
    /// Thêm một client WebSocket mới vào danh sách
    /// </summary>
    /// <param name="webSocket">Phiên kết nối WebSocket của client</param>
    public static void AddClient(WebSocket webSocket)
    {
        // Thêm kết nối vào ConcurrentBag
        _webSockets.Add(webSocket);
    }

    /// <summary>
    /// Loại bỏ một client WebSocket khỏi danh sách
    /// </summary>
    /// <param name="webSocket">Phiên kết nối WebSocket cần loại bỏ</param>
    public static void RemoveClient(WebSocket webSocket)
    {
        // Tạo ConcurrentBag mới, loại bỏ kết nối cần xoá bằng Except
        _webSockets = new ConcurrentBag<WebSocket>(_webSockets.Except(new[] { webSocket }));
    }

    /// <summary>
    /// Lấy toàn bộ client WebSocket hiện tại
    /// </summary>
    /// <returns>ConcurrentBag chứa tất cả WebSocket client</returns>
    public static ConcurrentBag<WebSocket> GetAllClients()
    {
        // Trả về danh sách tĩnh chứa kết nối WebSocket
        return _webSockets;
    }
}
