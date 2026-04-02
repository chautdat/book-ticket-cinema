using cinema.Models;

﻿namespace cinema.Services
{
    public interface CinemaService
    {
        public dynamic findAll();
        public bool update(Cinema cinema);
        public bool delete(int id);
        public bool create(Cinema cinema);
    }
}
