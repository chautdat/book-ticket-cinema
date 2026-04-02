using cinema.Models;

namespace cinema.Services
{
    public interface ChatService
    {
        public dynamic findChatByAccountId(int accountId);

        public Boolean newChat(Chat chat);

        public dynamic listUser();
    }
}
