
using DentaMatch.Services.Notifications;
using DentaMatch.Services.Notifications.IServices;

namespace DentaMatch.Middlewares
{
    public static class ApplicationBuilderExtension
    {
        public static void UseSqlTableDependency(this IApplicationBuilder applicationBuilder, string connectionString)
           
        {
            try
            {
                using (var scope = applicationBuilder.ApplicationServices.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var service = serviceProvider.GetService<ITableDependencyService>();
                    service?.SubscribeTableDependency(connectionString);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
