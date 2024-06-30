
using DentaMatch.Cache;
using DentaMatch.Models.Notifications;
using DentaMatch.Services.FireBase.IServices;
using DentaMatch.Services.Notifications.IServices;
using TableDependency.SqlClient;

namespace DentaMatch.Services.Notifications
{
    public class TableDependencyService : ITableDependencyService
    {
        private SqlTableDependency<UserNotifications> tableDependency;
        private readonly CacheItem _cache;
        private readonly IFirebaseService _firebase;

        public TableDependencyService(CacheItem cache, IFirebaseService firebase)
        {
            _cache = cache;
            _firebase = firebase;
        }

        public void SubscribeTableDependency(string connectionString)
        {
            tableDependency = new SqlTableDependency<UserNotifications>(connectionString);

            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(UserNotifications)} SqlTableDependency error: {e.Error.Message}");
        }

        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<UserNotifications> e)
        {
            try
            {
                if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
                {
                    var notification = e.Entity;
                    string token = (string)_cache.Retrieve(notification.UserName);
                    _firebase.SendMessageAsync(notification.Title, notification.Message, token);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
