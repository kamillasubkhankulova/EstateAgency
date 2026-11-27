using EstateAgency.Models;
using EstateAgency.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace EstateAgency
{
    public partial class MainWindow : Window
    {
        private readonly Employee _currentUser;
        private readonly IServiceProvider _services;

        public bool LogoutRequested { get; private set; }

        public MainWindow(Employee currentUser, IServiceProvider services)
        {
            InitializeComponent();

            _currentUser = currentUser;
            _services = services;

            UserFullNameText.Text = currentUser.FullName;
            UserRoleText.Text = currentUser.GetRoleName();
            WelcomeText.Text = $"Вы вошли как {currentUser.FullName}";
        }

        private void OnPropertiesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                WelcomePanel.Visibility = Visibility.Collapsed;
                ContentHost.Content = _services.GetRequiredService<PropertiesView>();
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Открытие раздела «Объекты»");
            }
        }

        private void OnClientsClick(object sender, RoutedEventArgs e) => ShowInDevelopment("Клиенты");

        private void OnDealsClick(object sender, RoutedEventArgs e) => ShowInDevelopment("Сделки");

        private void OnReportsClick(object sender, RoutedEventArgs e) => ShowInDevelopment("Отчёты");

        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            LogoutRequested = true;
            Close();

            ((App)Application.Current).ShowLogin();
        }

        private static void ShowInDevelopment(string section) =>
            MessageBox.Show($"Раздел «{section}» в разработке.",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}