using cinema.Models;                  // Import model Chat và Account (navigation properties)
using System;                         // Cho DateTime
using System.Linq;                    // Cho LINQ như Where, GroupBy

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ chat: lấy và thêm tin nhắn trong hệ thống
    /// </summary>
    public class ChatServiceImpl : ChatService
    {
        private readonly MyDbContext db;  // DbContext để truy vấn và lưu Chats

        public ChatServiceImpl(MyDbContext _db)
        {
            db = _db; // Dependency Injection: nhận MyDbContext từ container
        }

        /// <summary>
        /// Lấy toàn bộ cuộc trò chuyện của một user theo AccountId
        /// </summary>
        public dynamic findChatByAccountId(int accountId)
        {
            // Lọc các bản ghi Chat với AccountId, select các trường cần thiết và format thời gian
            return db.Chats
                .Where(c => c.AccountId == accountId)
                .Select(c => new
                {
                    Id = c.Id,
                    AccountId = c.AccountId,
                    Name = c.Account.Username,                         // Tên người chat (navigation property)
                    Message = c.Message,                              // Nội dung tin nhắn
                    Role = c.Role,                                    // Vai trò người gửi (user/admin)
                    Time = c.Time.ToString("dd/MM/yyyy HH:mm:ss"),  // Định dạng thời gian
                })
                .ToList(); // Thực thi truy vấn và trả về List
        }

        /// <summary>
        /// Lấy danh sách user đã chat cùng các tin nhắn tương ứng, gom nhóm theo AccountId
        /// </summary>
        public dynamic listUser()
        {
            // Group by AccountId để gom nhóm tin nhắn của cùng một user
            return db.Chats
                .GroupBy(c => c.AccountId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    // Lấy tên user từ bảng Accounts
                    Name = db.Accounts.Where(a => a.Id == g.Key).FirstOrDefault().Username,
                    // Danh sách tin nhắn của user đó
                    Messages = g.Select(c => new
                    {
                        Id = c.Id,
                        Message = c.Message,
                        Role = c.Role,
                        Time = c.Time.ToString("dd/MM/yyyy HH:mm:ss"),
                    }).ToList()
                })
                .ToList(); // Trả về List các nhóm chat
        }

        /// <summary>
        /// Tạo mới một tin nhắn chat và lưu vào database
        /// </summary>
        public bool newChat(Chat chat)
        {
            // Luôn gán thời gian server để tránh lỗi parse/format từ client
            chat.Time = DateTime.Now;
            db.Chats.Add(chat);                   // Thêm chat vào DbSet
            return db.SaveChanges() > 0;          // Lưu và trả về true nếu thành công
        }
    }
}
