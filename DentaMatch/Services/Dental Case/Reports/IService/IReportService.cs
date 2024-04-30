using DentaMatch.ViewModel;

namespace DentaMatch.Services.Dental_Case.Reports.IService
{
    public interface IReportService
    {
        Task<AuthModel> Report(string caseId, string doctorId);
    }
}
