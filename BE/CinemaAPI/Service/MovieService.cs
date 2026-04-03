using cinema.Models;

namespace cinema.Services;

public interface MovieService
{
    public dynamic findAll(bool status, string date, int cinemaId);

    public dynamic findMovie(bool status, string date, int cinemaId, int movieId);

    public dynamic findAll(bool status);
    public dynamic findById(int id);
    public Movie findById1(int id);

    public Boolean create(Movie movie);
    public Boolean update(Movie movie);
}
