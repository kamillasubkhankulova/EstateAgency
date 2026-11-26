using System.Windows;
using EstateAgency.Models;

namespace EstateAgency
{
    public partial class MainWindow : Window
    {
        private readonly Employee _currentUser;

        /// <summary>
        /// закрыли именно через «Выйти из аккаунта»
        /// </summary>
        public bool LogoutRequested { get; private set; }

        public MainWindow(Employee currentUser)
        {
            InitializeComponent();

            _currentUser = currentUser;

            UserFullNameText.Text = currentUser.FullName;
            UserRoleText.Text = currentUser.GetRoleName();
            WelcomeText.Text = $"Вы вошли как {currentUser.FullName}";
        }

        private void OnPropertiesClick(object sender, RoutedEventArgs e) => ShowInDevelopment("Объекты");

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