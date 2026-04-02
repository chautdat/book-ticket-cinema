using cinema.Models;

namespace cinema.Services
{
    public interface FollowService
    {
        public Boolean create(Follow follow);

        public dynamic findById(int id);

        public dynamic findAll();
    }
}
