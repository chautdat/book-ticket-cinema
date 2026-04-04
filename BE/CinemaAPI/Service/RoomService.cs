using cinema.Models;

namespace cinema.Services
{
    public interface RoomService
    {
        public dynamic findAll();
        public Boolean create(Room room);

        public Boolean update(Room room);

        public Room findById(int id);

        public dynamic findById1(int id);

        public dynamic findAllByCinema(int cinemaId);
    }
}
