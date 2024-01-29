using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.ViewModel.Authentication;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Authentication
{
    public class DoctorRepository:IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration Configuration;
        private readonly ApplicationDbContext _db;

        public DoctorRepository(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext db)
        {
            _userManager = userManager;
            Configuration = configuration;
            _db = db;
        }

        public Task<AuthModel> SignInAsync(UserSignInVM model)
        {
            throw new NotImplementedException();
        }
    }
}
