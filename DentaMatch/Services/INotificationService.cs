using DentaMatch.ViewModel;

namespace DentaMatch.Services
{
    public interface INotificationService
    {
        AuthModel<NotificationVM> AddNotification(NotificationVM notification);
    }
}
