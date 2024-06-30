using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DentaMatch.Models.Notifications
{
    public class UserNotifications
    {
        [Key]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; } = null!;
        public string Title { get; set; } = null!;
        public DateTime NotificationDateTime { get; set; }
    }
}
