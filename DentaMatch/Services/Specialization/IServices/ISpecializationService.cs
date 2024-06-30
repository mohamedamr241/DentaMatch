using DentaMatch.ViewModel;
using DentaMatch.ViewModel.Specialization;

namespace DentaMatch.Services.Specialization.IServices
{
    public interface ISpecializationService
    {
        AuthModel RequestSpecialization(SpecialzationVM model, string doctorId);
    }
}
