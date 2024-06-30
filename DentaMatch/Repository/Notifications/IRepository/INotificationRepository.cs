using DentaMatch.Models;
using DentaMatch.Models.Notifications;

namespace DentaMatch.Repository.Notifications.IRepository
{
    public interface INotificationRepository : IRepository<UserNotifications>
    {
        void Save();
    }
}
