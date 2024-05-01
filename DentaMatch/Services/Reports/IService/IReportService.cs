using DentaMatch.ViewModel;

namespace DentaMatch.Services.Reports.IService
{
    public interface IReportService
    {
        Task<AuthModel> Report(string userId, string caseId);
    }
}
