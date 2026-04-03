using cinema.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace cinema.Services
{
    public interface RatingService
    {
        public dynamic findAll(int movieId);


        public dynamic findAll();

        public dynamic findAll(int movieId, bool status);

        public dynamic create(Rating rating);

        public double avgByMovieId(int movieId);

    }
}
