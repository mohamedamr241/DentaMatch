using DentaMatch.Data;
using DentaMatch.Models;
using DentaMatch.Models.Notifications;
using DentaMatch.Repository.Dental_Case.IRepository;
using DentaMatch.Repository.Notifications.IRepository;

namespace DentaMatch.Repository.Notifications
{
    public class NotificationRepository : Repository<UserNotifications>, INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        public NotificationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
