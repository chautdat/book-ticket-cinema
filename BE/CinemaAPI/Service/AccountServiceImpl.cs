using cinema.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace cinema.Services
{
    /// <summary>
    /// Triển khai các phương thức của AccountService, tương tác trực tiếp với DbContext
    /// </summary>
    public class AccountServiceImpl : AccountService
    {
        private readonly MyDbContext db; // DbContext cho truy vấn và lưu dữ liệu

        public AccountServiceImpl(MyDbContext _db)
        {
            db = _db; // Dependency Injection: nhận MyDbContext từ container
        }

        /// <summary>
        /// Kiểm tra đăng nhập bằng email và password
        /// </summary>
        public bool checkLogin(string email, string password)
        {
            try
            {
                var account = db.Accounts.SingleOrDefault(a => a.Email == email);
                if (account != null)
                {
                    return BCrypt.Net.BCrypt.Verify(password, account.Password);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin tài khoản theo username
        /// </summary>
        public dynamic findByUsername(string username)
        {
            return db.Accounts
                .Where(acc => acc.Username == username)
                .Select(acc => new
                {
                    acc.Id,
                    acc.Username,
                    acc.Email,
                    acc.Password,
                    acc.Phone,
                    acc.Gender,
                    acc.Birthday,
                    acc.Created,
                    acc.Verify,
                    acc.Securitycode,
                    acc.Role
                })
                .FirstOrDefault();
        }

        /// <summary>
        /// Lấy thông tin tài khoản theo email
        /// </summary>
        public dynamic findByEmail(string email)
        {
            return db.Accounts
                .Where(acc => acc.Email == email)
                .Select(acc => new
                {
                    acc.Id,
                    acc.Username,
                    acc.Email,
                    acc.Password,
                    acc.Phone,
                    acc.Gender,
                    acc.Birthday,
                    acc.Created,
                    acc.Verify,
                    acc.Securitycode,
                    acc.Role
                })
                .FirstOrDefault();
        }

        /// <summary>
        /// Đăng ký tài khoản mới: hash mật khẩu, lưu vào database
        /// </summary>
        public bool register(Account account)
        {
            // Kiểm tra trùng email/username
            if (db.Accounts.Any(a => a.Email == account.Email))
            {
                throw new InvalidOperationException("Email đã tồn tại");
            }
            if (db.Accounts.Any(a => a.Username == account.Username))
            {
                throw new InvalidOperationException("Username đã tồn tại");
            }
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            db.Accounts.Add(account);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Cập nhật thông tin tài khoản (update toàn bộ object), hash lại mật khẩu nếu có mật khẩu mới
        /// </summary>
        public bool update(Account account)
        {
            var existing = db.Accounts.FirstOrDefault(a => a.Id == account.Id);
            if (existing == null)
            {
                throw new InvalidOperationException("Tài khoản không tồn tại");
            }

            existing.Username = account.Username;
            existing.Email = account.Email;
            existing.Phone = account.Phone;
            existing.Gender = account.Gender;
            existing.Birthday = account.Birthday;
            existing.Securitycode = account.Securitycode;
            existing.Role = account.Role;
            existing.Verify = account.Verify;

            // Hash mật khẩu mới nếu được truyền lên; để trống sẽ giữ mật khẩu cũ
            if (!string.IsNullOrWhiteSpace(account.Password))
            {
                existing.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            }

            db.Update(existing);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Hard-delete tài khoản: XÓA HOÀN TOÀN khỏi database
        /// Lưu ý: Chỉ xóa được nếu account chưa có data liên quan (Booking, Rating, Follow, Chat)
        /// </summary>
        public bool delete(int id)
        {
            var account = db.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null)
            {
                return false;
            }

            // XÓA CỨNG (HARD DELETE) - Xóa luôn trong database
            // Cascade delete sẽ tự động xóa các bản ghi liên quan nếu được cấu hình
            db.Accounts.Remove(account);
            
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Lấy toàn bộ tài khoản
        /// </summary>
        public dynamic findAll()
        {
            return db.Accounts
                .Select(a => new
                {
                    a.Id,
                    a.Username,
                    a.Password,
                    a.Role,
                    a.Created,
                    a.Verify,
                    a.Email,
                    a.Phone,
                    a.Gender,
                    a.Birthday,
                    Sercuritycode = a.Securitycode
                })
                .ToList();
        }

        public async Task<Account> GetByEmailAsync(string email)
        {
            return await db.Accounts.SingleOrDefaultAsync(a => a.Email == email);
        }

        public async Task<bool> CreateAsync(Account account)
        {
            db.Accounts.Add(account);
            return await db.SaveChangesAsync() > 0;
        }

        public dynamic findByAccountId(int id)
        {
            return db.Accounts
                .Where(acc => acc.Id == id)
                .Select(acc => new
                {
                    acc.Id,
                    acc.Username,
                    acc.Email,
                    acc.Password,
                    acc.Phone,
                    acc.Gender,
                    acc.Birthday,
                    acc.Created,
                    acc.Verify,
                    acc.Role,
                    Sercuritycode = acc.Securitycode
                })
                .FirstOrDefault();
        }
    }
}
