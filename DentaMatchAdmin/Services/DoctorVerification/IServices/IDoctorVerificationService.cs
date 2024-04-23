using DentaMatch.ViewModel.Authentication;
using DentaMatch.ViewModel;

namespace DentaMatchAdmin.Services.DoctorVerification.IServices
{
    public interface IDoctorVerificationService
    {
        Task<AuthModel<List<DoctorResponseVM>>> GetUnverifiedDoctorsAsync();
        Task<AuthModel> VerifyDoctorIdentity(string doctorId, bool isIdentityVerified);
    }
}
