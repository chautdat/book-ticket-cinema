using cinema.Models;

namespace cinema.Services
{
    public interface AccountService
    {
        public bool checkLogin(string email, string password);

        public bool register(Account account);

        public bool update(Account account);
        public bool delete(int id);

        public dynamic findByAccountId(int id);
        public dynamic findAll();

        public dynamic findByEmail(string email);

        Task<Account> GetByEmailAsync(string email);
        Task<bool> CreateAsync(Account account);
    }
}
