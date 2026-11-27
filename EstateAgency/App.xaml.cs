using System.Windows;
using System.Windows.Threading;
using EstateAgency.Data;
using EstateAgency.Models;
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
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Services = ConfigureServices();

            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            ShowLogin();

        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(ConnectionString));

            services.AddScoped<AuthService>();
            services.AddScoped<PropertyService>();
            services.AddScoped<ClientService>();
            services.AddScoped<DealService>();
            services.AddTransient<PropertiesView>();

            services.AddTransient<LoginWindow>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Показывает окно входа. При успехе открывает главное окно
        /// </summary>
        public void ShowLogin()
        {
            IServiceScope scope = Services.CreateScope();

            var loginWindow = scope.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.ShowDialog();

            Employee? user = loginWindow.LoggedInUser;
            scope.Dispose();

            if (user is null)
            {
                Shutdown();
                return;
            }

            ShowMainWindow(user);
        }

        private void ShowMainWindow(Employee user)
        {
            IServiceScope scope = Services.CreateScope();

            var mainWindow = ActivatorUtilities.CreateInstance<MainWindow>(scope.ServiceProvider, user);

            mainWindow.Closed += (_, _) =>
            {
                scope.Dispose();

                if (!mainWindow.LogoutRequested)
                    Shutdown(); 
            };

            MainWindow = mainWindow;
            mainWindow.Show();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionHandler.Handle(e.Exception, "UI-поток");
            e.Handled = true;
        }

        private void OnDomainUnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
                ExceptionHandler.Handle(exception, "Фоновый поток");
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            ExceptionHandler.Handle(e.Exception, "Task");
            e.SetObserved();
        }
    }
}