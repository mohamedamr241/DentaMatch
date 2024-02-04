using DentaMatch.Data;
using DentaMatch.Repository.Authentication.IRepository;

namespace DentaMatch.Repository.Authentication
{
    public class UserRepository<T> : Repository<T>, IUserRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
