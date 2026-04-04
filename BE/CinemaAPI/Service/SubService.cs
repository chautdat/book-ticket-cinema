using cinema.Models;

namespace cinema.Services
{

        public interface SubService
        {
            public dynamic findAll();
            public Boolean create(Sub sub);

            public Boolean update(Sub sub);

            public Sub findById(int id);

            public dynamic findById1(int id);
        }
}
