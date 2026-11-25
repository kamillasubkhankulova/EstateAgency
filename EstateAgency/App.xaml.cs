using System.Windows;
using EstateAgency.Data;
using EstateAgency.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EstateAgency
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public static string ConnectionString { get; } =
            "Server=(localdb)\\MSSQLLocalDB;Database=EstateAgencyDB;Trusted_Connection=True;";

        protected override void OnStartup(StartupEventArgs e)
        {
            var collection = new ServiceCollection();

            collection.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(ConnectionString));

            collection.AddScoped<AuthService>();
            collection.AddScoped<PropertyService>();
            collection.AddScoped<ClientService>();
            collection.AddScoped<DealService>();

            Services = collection.BuildServiceProvider();

            base.OnStartup(e);
        }
        public static IServiceScope CreateScope() => Services.CreateScope();
    }
}