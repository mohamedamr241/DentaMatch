using DentaMatch.Data;
using DentaMatch.Repository.Authentication.IRepository;

namespace DentaMatch.Repository.Authentication
{
    public class UserRepository<T> : Repository<T>, IUserRepository<T> where T : class
    {
        public UserRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
