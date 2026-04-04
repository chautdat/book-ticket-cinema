using cinema.Models;

namespace cinema.Services
{
    public interface ShowTimeService
    {
        public dynamic findById(int id);

        public dynamic checkSeat(int id);

        public dynamic findMovie(bool status, string date, int cinemaId, int movieId);

        public dynamic findAll();

        public dynamic findAllByCinemaId(int cinemaId);

        public Showtime findById1(int id);
        public Boolean create(Showtime showtime);

        public Boolean update(Showtime showtime);

        public dynamic listMovieTomorrow();

        public bool deleteAll();

        public bool createRandom();
    }
}
