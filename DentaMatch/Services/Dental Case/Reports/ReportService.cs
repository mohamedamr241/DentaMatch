using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Reports;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Services.Authentication.IServices;
using DentaMatch.Services.Dental_Case.IServices;
using DentaMatch.Services.Dental_Case.Reports.IService;
using DentaMatch.ViewModel;

namespace DentaMatch.Services.Dental_Case.Reports
{
    public class ReportService : IReportService
    {
        private readonly IDentalUnitOfWork _dentalunitOfWork;
        private readonly IAuthService _authService;
        private readonly IDentalCaseService _dentalCaseService;
        public ReportService(IDentalUnitOfWork dentalunitOfWork, IAuthService authService, IDentalCaseService dentalCaseService)
        {
            _dentalunitOfWork = dentalunitOfWork;
            _authService = authService;
            _dentalCaseService = dentalCaseService; 
        }

        public async Task<AuthModel> Report(string caseId, string doctorId)
        {
            try
            {
                var dentalCase = _dentalunitOfWork.DentalCaseRepository.Get(u => u.Id == caseId, "Patient.User");
                if (dentalCase == null)
                {
                    return new AuthModel { Success = false, Message = "Dental Case Not Found" };
                }

                var existingReport = _dentalunitOfWork.DentalCaseRepository.Report.Get(r => r.CaseId == caseId && r.DoctorId == doctorId);

                if (existingReport != null)
                {
                    return new AuthModel { Success = false, Message = "Doctor has already reported this case" };
                }

                var report = new Report
                {
                    CaseId = caseId,
                    DoctorId = doctorId,
                    PatientId = dentalCase.PatientId,
                    ReportTimestamp = DateTime.UtcNow
                };

                _dentalunitOfWork.DentalCaseRepository.Report.Add(report);
                _dentalunitOfWork.Save();

                var reportCount = _dentalunitOfWork.DentalCaseRepository.Report.Count(r => r.PatientId == dentalCase.PatientId);

                int reportThreshold = 2; 

                if (reportCount >= reportThreshold)
                {
                    var res = _authService.BlockAccount(dentalCase.Patient.User);
                    if (!res.Result.Success)
                    {
                        return res.Result;
                    }

                    var reports = _dentalunitOfWork.DentalCaseRepository.Report.GetAll(u=>u.CaseId == caseId);
                    _dentalunitOfWork.DentalCaseRepository.Report.RemoveRange(reports);

                    var DentalCases = _dentalunitOfWork.DentalCaseRepository.GetAll(u=>u.PatientId == dentalCase.PatientId);
                    foreach (var DentalCase in DentalCases)
                    {
                        _dentalCaseService.DeleteCase(DentalCase.Id);
                    }

                    return new AuthModel { Success = true, Message = "Dental Case Reported Successfully & Patient account has been blocked due to excessive reports" };

                }

                return new AuthModel { Success = true, Message = "Dental Case Reported Successfully" };

            }
            catch (Exception ex)
            {
                return new AuthModel { Success = false, Message = $"Error Reporting dental case: {ex.Message}" };
            }
        }
    }
}
