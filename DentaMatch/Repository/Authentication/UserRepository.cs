using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;

namespace DentaMatch.Repository.Authentication
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db)
        {
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

        public void UpdateProfilePicture(ApplicationUser user, string ImagePathLink, string ImagePath)
        {
            if (user != null)
            {
                user.ProfileImage = ImagePath;
                user.ProfileImageLink = ImagePathLink;
            }
        }
        public bool UpdateUserAccount(ApplicationUser userDetails, ApplicationUser user)
        {
            if (user != null)
            {
                user.Age = userDetails.Age;
                user.Email = userDetails.Email;
                user.FullName = userDetails.FullName;
                user.Gender = userDetails.Gender;
                user.PhoneNumber = userDetails.PhoneNumber;
                user.UserName = userDetails.UserName;
                user.City = userDetails.City;
                user.NormalizedUserName = (userDetails.UserName).ToUpper();
                user.NormalizedEmail = (userDetails.Email).ToUpper();
                return true;
            }
            return false;
        }


    }
}
