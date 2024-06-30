using DentaMatch.Helpers;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Models;
using DentaMatch.Models.Doctor_Models;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Repository.Specialization.IRepository;
using DentaMatch.Services.Specialization.IServices;
using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Specialization;
using System.Numerics;

namespace DentaMatch.Services.Specialization
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository _specializationRepo;
        private readonly IDentalUnitOfWork _dentalunitOfWork;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly AppHelper _appHelper;
        private readonly IConfiguration _configuration;
        public SpecializationService(IConfiguration configuration, AppHelper appHelper, ISpecializationRepository specializationRepo, IDentalUnitOfWork dentalunitOfWork, IAuthUnitOfWork authUnitOfWork) 
        {
            _specializationRepo = specializationRepo;
            _dentalunitOfWork = dentalunitOfWork;
            _authUnitOfWork = authUnitOfWork;
            _appHelper = appHelper;
            _configuration = configuration;
        }
        public AuthModel RequestSpecialization(SpecialzationVM model, string doctorId)
        {
            try
            {
                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.UserId == doctorId);
                if (doctor == null)
                {
                    return new AuthModel { Success = false, Message = "User not found!" };
                }
                
                UpsertSpecializationImage(model.RequiredFile, doctor.Id, model.Specialization);
                
                return new AuthModel { Success = true, Message = "Your request is submitted, please wait for admin approval" };
            }
            catch(Exception ex)
            {
                return new AuthModel { Success = false, Message = $"Error in requesting {ex}" };
            }
        }
        private void UpsertSpecializationImage(IFormFile Image, string doctorId, string Specialization)
        {

            if (Image is not null)
            {
                string ImagePath = Path.Combine("wwwroot", "Images", "Doctor", "Specialization");
                string ImageName = _appHelper.SaveImage(Image, ImagePath);
                string imagePath = $"{_configuration["ImgUrl"]}" + Path.Combine("Images", "Doctor", "Specialization", ImageName);

                var doctorSpecialization = new DoctorSpecializationRequests
                {
                    Id = Guid.NewGuid().ToString(),
                    DoctorId = doctorId,
                    Specialization = Specialization,
                    IsVerified = false,
                    Image = Path.Combine(ImagePath, ImageName),
                    ImageLink = imagePath
                };
                _specializationRepo.Add(doctorSpecialization);
                _specializationRepo.Save();
            }
        }
    }
}
