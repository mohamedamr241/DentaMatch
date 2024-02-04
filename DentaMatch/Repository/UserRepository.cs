using DentaMatch.Data;
using DentaMatch.Repository.IRepository;

namespace DentaMatch.Repository
{
    public class UserRepository<T> : Repository<T>, IUserRepository<T> where T: class
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
