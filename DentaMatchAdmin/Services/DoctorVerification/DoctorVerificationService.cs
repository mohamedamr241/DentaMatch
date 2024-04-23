using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel;
using DentaMatchAdmin.Services.DoctorVerification.IServices;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Mail.IServices;

namespace DentaMatchAdmin.Services.DoctorVerification
{
    public class DoctorVerificationService : IDoctorVerificationService
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly IAuthDoctorService _doctorService;
        private readonly IMailService _mailService;

        public DoctorVerificationService(IAuthUnitOfWork authUnitOfWork, IAuthDoctorService doctorService, IMailService mailService)
        {
            _authUnitOfWork = authUnitOfWork;
            _doctorService = doctorService;
            _mailService = mailService;
        }
        public async Task<AuthModel<List<DoctorResponseVM>>> GetUnverifiedDoctorsAsync()
        {
            return await _doctorService.GetUnverifiedDoctorsAsync();
        }

        public async Task<AuthModel> VerifyDoctorIdentity(string doctorId, bool isIdentityVerified)
        {
            try
            {
                var doctor = _authUnitOfWork.DoctorRepository.Get(u => u.Id == doctorId, "User");
                if (doctor is null)
                {
                    return new AuthModel { Success = false, Message = "Doctor Not Found" };
                }
                if (!isIdentityVerified)
                {
                    
                    if (doctor.User.EmailConfirmed)
                    {
                        await _mailService.SendEmailAsync(
                            doctor.User.Email,
                            "Identity Verification Result",
                            $"<h2>Identity Status</h2>" +
                            $"<p>Hello Dr/{doctor.User.FullName.Split(' ')[0]},</p>" +
                            $"<p>We regret to inform you that your identity verification has been <b>Rejected</b>.</p>" +
                            $"<p>You can contact us for further details or to provide additional information for reconsideration.</p>" +
                            $"<p>Thank you for your understanding.</p>" +
                            $"<p>Best regards,</p>" +
                            $"<p>DentaMatch Team.</p>"
                        );
                    }
                    await _doctorService.DeleteAccount(doctor.UserId);
                    return new AuthModel { Success = true, Message = "Doctor Identity Is Rejected" };
                }

                _authUnitOfWork.DoctorRepository.UpdateDoctorIdentityStatus(doctor, isIdentityVerified);
                _authUnitOfWork.Save();

                if (doctor.User.EmailConfirmed)
                {
                    await _mailService.SendEmailAsync(
                        doctor.User.Email,
                        "Identity Verification Result",
                        $"<h2>Identity Status</h2>" +
                        $"<p>Hello Dr/{doctor.User.FullName.Split(' ')[0]},</p>" +
                        $"<p>We are pleased to inform you that your identity has been <b>Accepted</b>. You can now login to your account and access all features.</p>" +
                        $"<p>Thank you for your patience and cooperation.</p>" +
                        $"<p>Best regards,</p>" +
                        $"<p>DentaMatch Team.</p>"
                    );
                }
                return new AuthModel { Success = true, Message = "Doctor Identity Is Accepted" };
            }
            catch (Exception error)
            {
                return new AuthModel { Success = false, Message = $"{error.Message}" };
            }
        }
    }
}
