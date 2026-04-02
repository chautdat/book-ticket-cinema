using cinema.Models;       // Import các model dữ liệu liên quan đến Chat
using cinema.Services;     // Import service xử lý nghiệp vụ chat
using Microsoft.AspNetCore.Mvc; // Cung cấp Controller và attribute cho API

namespace cinema.Controllers
{
    [Route("api/chat")]              // Đặt route cơ sở cho các endpoint liên quan đến chat
    public class ChatController : Controller
    {
        private readonly ChatService _chatService; // Service quản lý chat

        public ChatController(ChatService chatService)
        {
            _chatService = chatService; // Dependency Injection: lấy instance ChatService từ container
        }

        [HttpGet("findChatByAccountId/{accountId}")] // GET /api/chat/findChatByAccountId/{accountId}
        [Produces("application/json")]                // Trả về dữ liệu JSON
        public IActionResult findAll(int accountId)     // Tham số lấy từ URL
        {
            try
            {
                // Gọi service tìm tất cả cuộc trò chuyện của user
                var chats = _chatService.findChatByAccountId(accountId);
                return Ok(chats);  // Trả về danh sách chat
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, trả về BadRequest kèm thông báo
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("newChat")]         // POST /api/chat/newChat
        [Produces("application/json")]// Trả về JSON
        [Consumes("application/json")]// Nhận JSON từ request body
        public IActionResult newChat([FromBody] Chat chat) // Bind payload vào model Chat
        {
            try
            {
                // Gọi service tạo cuộc trò chuyện mới
                bool status = _chatService.newChat(chat);
                // Trả về trạng thái và đối tượng chat vừa tạo
                return Ok(new { Status = status, Chat = chat });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("listUser")]         // GET /api/chat/listUser
        [Produces("application/json")]// Trả về JSON
        public IActionResult listUser()
        {
            try
            {
                // Gọi service lấy danh sách người dùng đang tham gia chat
                var users = _chatService.listUser();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
