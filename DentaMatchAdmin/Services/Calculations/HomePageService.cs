using DentaMatchAdmin.Services.Calculations.IServices;
using DentaMatchAdmin.ViewModels;
using DentaMatch.Repository.Authentication.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using Microsoft.Extensions.Caching.Memory;
using DentaMatchAdmin.Cache;

namespace DentaMatchAdmin.Services.Calculations
{
    public class HomePageService : IHomePageService
    {
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly IDentalUnitOfWork _DentalUnitOfWork;
        private readonly CacheItem _cache;

        public HomePageService(IAuthUnitOfWork authUnitOfWork, IDentalUnitOfWork DentalUnitOfWork, CacheItem cache)
        {  
            _authUnitOfWork = authUnitOfWork;
            _DentalUnitOfWork = DentalUnitOfWork;
            _cache = cache;
        }
        private int NumOfDoctors()
        {
            return _authUnitOfWork.DoctorRepository.Count();
        }
        private int NumOfPatients()
        {
            return _authUnitOfWork.PatientRepository.Count();
        }
        private int NumOfTreatments()
        {
            return _DentalUnitOfWork.DentalCaseRepository.Count(u => u.IsAssigned == true);
        }
        private double DoctorAccountGrossPercent(int NumOfAccounts)
        {
            DateTime lastWeek = (DateTime)_cache.Retrieve("week");
            return NumOfAccounts==0? 0:(_authUnitOfWork.DoctorRepository.Count(u => u.TimeStamp.Date >= lastWeek.Date) / NumOfAccounts) * 100;
        }
        private double PatientAccountGrossPercent(int NumOfAccounts)
        {
            DateTime lastWeek = (DateTime)_cache.Retrieve("week");
            return NumOfAccounts == 0 ? 0 : (_authUnitOfWork.PatientRepository.Count(u => u.TimeStamp.Date >= lastWeek.Date) / NumOfAccounts) * 100;
        }
        private double TreatmentstGrossPercent(int NumOfTreatments)
        {
            DateTime lastWeek = (DateTime)_cache.Retrieve("week");
            return NumOfTreatments == 0 ? 0 : (_DentalUnitOfWork.DentalCaseRepository.Count(u => u.IsAssigned == true &&  u.TimeStamp.Date >= lastWeek.Date) / NumOfTreatments) * 100;
        }
        private int NumOfNewCustomerPerWeek()
        {
            DateTime lastWeek = (DateTime)_cache.Retrieve("week");
            return _authUnitOfWork.UserRepository.Count(u => u.TimeStamp.Date >= lastWeek.Date);
        }
        private int NumOfCurrentCustomerPerWeek()
        {
            DateTime lastWeek = (DateTime)_cache.Retrieve("week");
            return _authUnitOfWork.UserRepository.Count(u => u.TimeStamp.Date < lastWeek.Date);
        }
        private double AccountsGrossLastYear()
        {
            DateTime lastWeek = (DateTime)_cache.Retrieve("week");
            int AllUserOfLastYears = _authUnitOfWork.UserRepository.Count(u => u.TimeStamp.Year <= (lastWeek.Year));
            return AllUserOfLastYears == 0 ? 0 :(_authUnitOfWork.UserRepository.Count(u => u.TimeStamp.Year == (lastWeek.Year)) / _authUnitOfWork.UserRepository.Count(u => u.TimeStamp.Year <= (lastWeek.Year)))*100;
        }
        private int AccountsLastYearMonthes(int month)
        {
            DateTime lastWeek = (DateTime)_cache.Retrieve("week");
            return _authUnitOfWork.UserRepository.Count(u => u.TimeStamp.Year == (lastWeek.Year) && u.TimeStamp.Month == month);
        }
        public HomePageVM PageCalculations()
        {
            if(_cache.Retrieve("week") == null)
            {
                _cache.storeInDays("week", DateTime.UtcNow, 7);
            }
            int NumOfDoc = NumOfDoctors();
            int NumOfPatient = NumOfPatients();
            int numOfTreatment = NumOfTreatments();

            int newUsers = NumOfNewCustomerPerWeek();
            int CurrentUsers =  NumOfCurrentCustomerPerWeek();
            double weekStatus = (CurrentUsers + newUsers)==0? 0:(newUsers / (CurrentUsers + newUsers)) * 100;

            var Response = new HomePageVM()
            {
                TotalDoctors = NumOfDoc,
                TotalPatients = NumOfPatient,
                TotalTreatments = numOfTreatment,
                DoctorGrossPercentPerWeek = DoctorAccountGrossPercent(NumOfDoc),
                PatientGrossPercentPerWeek = PatientAccountGrossPercent(NumOfPatient),
                TreatmentsPercentPerWeek = TreatmentstGrossPercent(numOfTreatment),
                CurrentUserPerWeek = CurrentUsers,
                NewUserPerWeek = newUsers,
                TotalUserPerWeek = CurrentUsers + newUsers,
                WeeklyStatusOfUsers = weekStatus,
                TotalAccounts = NumOfDoc + NumOfPatient,
                AccountsGrossLastYear = AccountsGrossLastYear(),
                January = AccountsLastYearMonthes(1),
                February = AccountsLastYearMonthes(2),
                March = AccountsLastYearMonthes(3),
                April = AccountsLastYearMonthes(4),
                May = AccountsLastYearMonthes(5),
                June = AccountsLastYearMonthes(6),
                July = AccountsLastYearMonthes(7),
                August = AccountsLastYearMonthes(8),
                September = AccountsLastYearMonthes(9),
                October = AccountsLastYearMonthes(10),
                November = AccountsLastYearMonthes(11),
                December = AccountsLastYearMonthes(12),
            };  

            return Response;
        }
    }
}
