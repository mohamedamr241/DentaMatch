using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication.Patient;
using DentaMatch.ViewModel.Authentication.Request;

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

        public void UpdateProfilePicture(ApplicationUser user, string? ImagePath = null, string? ImagePathLink = null)
        {
            if (user != null)
            {
                user.ProfileImage = ImagePath;
                user.ProfileImageLink = ImagePathLink;
            }
        }
        public void UpdateUserAccount(ApplicationUser user, UserUpdateRequestVM updatedUser)
        {
            if (user != null)
            {
                if (user.Email != updatedUser.Email)
                {
                    user.EmailConfirmed = false; 
                    user.Email = updatedUser.Email;
                }
                user.Age = updatedUser.Age;
                user.FullName = updatedUser.FullName;
                user.Gender = updatedUser.Gender;
                user.PhoneNumber = updatedUser.PhoneNumber;
                user.UserName = updatedUser.userName;
                user.City = updatedUser.City;
            }
        }
    }
}
