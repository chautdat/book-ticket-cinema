using cinema.Models;

namespace cinema.Services
{

        public interface SeatService
        {
            public dynamic findAll();
            public Boolean create(Seat seat);

            public Boolean update(Seat seat);

            public Seat findById(int id);

            public dynamic findById1(int id);
        }
}
