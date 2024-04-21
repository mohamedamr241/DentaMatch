namespace DentaMatchAdmin.ViewModels
{
    public class HomePageVM
    {
        public int TotalDoctors { get; set; } = 0;
        public double DoctorGrossPercentPerWeek { get; set; } = 0;
        public int TotalPatients { get; set; } = 0;
        public double PatientGrossPercentPerWeek { get; set; } = 0;
        public int TotalTreatments { get; set; } = 0;
        public double TreatmentsPercentPerWeek { get; set; } = 0;

        public int CurrentUserPerWeek { get; set;} = 0;
        public int NewUserPerWeek { get; set; } = 0;
        public int TotalUserPerWeek { get; set; } = 0;
        public double WeeklyStatusOfUsers { get; set; } = 0;

        public int TotalAccounts { get; set; } = 0;
        public double AccountsGrossLastYear { get; set; } = 0;

        public int January { get; set; } = 0;
        public int February { get; set; } = 0;
        public int March { get; set; } = 0;
        public int April { get; set; } = 0;
        public int May { get; set; } = 0;
        public int June { get; set; } = 0;
        public int July { get; set; } = 0;
        public int August { get; set; } = 0;
        public int September { get; set; } = 0;
        public int October { get; set; } = 0;
        public int November { get; set; } = 0;
        public int December { get; set; } = 0;

    }
}
