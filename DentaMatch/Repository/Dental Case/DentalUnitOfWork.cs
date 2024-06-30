using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Repository.Case_Appointment;
using DentaMatch.Repository.Case_Appointment.IRepository;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Repository.Notifications;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Dental_Case
{
    public class DentalUnitOfWork : IDentalUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IDentalCaseRepository DentalCaseRepository { get; private set; }
        public ICaseAppointmentRepository CaseAppointmentRepository { get; private set; }
        public IDentalCaseCommentRepository CaseCommentRepository { get; private set; }
        public IDentalCaseProgressRepository CaseProgressRepository { get; private set; }
        public NotificationRepository notifications { get; private set; }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public DentalUnitOfWork(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            UserManager = userManager;
            DentalCaseRepository = new DentalCaseRepository(_db);
            CaseAppointmentRepository = new CaseAppointmentRepository();
            CaseCommentRepository = new DentalCaseCommentRepository(_db);
            CaseProgressRepository = new DentalCaseProgressRepository(_db);
            notifications = new NotificationRepository(_db);

        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
