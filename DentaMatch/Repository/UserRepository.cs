using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.IRepository;

namespace DentaMatch.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository<ApplicationUser>
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void UpdateVerificationCode(ApplicationUser user, string verificationCode, bool isVerified = false)
        {
            if (user != null)
            {
                user.VerificationCode = verificationCode;
                user.VerificationCodeTimeStamp = DateTime.Now;
                user.IsVerified = isVerified;
            }
        }
    }
}
