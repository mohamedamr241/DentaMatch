using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;

namespace DentaMatch.Repository.Authentication
{
    public class UserManagerRepository : Repository<ApplicationUser>, IUserManagerRepository
    {
        private readonly ApplicationDbContext _db;
        public UserManagerRepository(ApplicationDbContext db) : base(db)
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
