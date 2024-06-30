using DentaMatch.Models.Notifications;
using DentaMatch.Repository.Notifications.IRepository;
using DentaMatch.ViewModel;

namespace DentaMatch.Services
{
    public class NotificationServices : INotificationService
    {
        private readonly INotificationRepository _notification;
        public NotificationServices(INotificationRepository notification)
        {
            _notification = notification;
        }
        public AuthModel<NotificationVM> AddNotification(NotificationVM notification)
        {
            try
            {
                var UserNotification = new UserNotifications
                {
                    Id = notification.Id,
                    UserName = notification.UserName,
                    Message = notification.message,
                    NotificationDateTime = DateTime.Now,
                };
                _notification.Add(UserNotification);
                _notification.Save();
                return new AuthModel<NotificationVM> { Success = true, Message = "SUCCESS", Data = notification };
            }
            catch(Exception ex)
            {
                return new AuthModel<NotificationVM>() { Success = false, Message = $"error in notification services: {ex}" };
            }
        }
    }
}
