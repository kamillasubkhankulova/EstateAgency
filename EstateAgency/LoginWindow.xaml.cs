using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EstateAgency.Models;
using EstateAgency.Services;

namespace EstateAgency
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Заполняется только при успешном входе
        /// </summary>
        public Employee? LoggedInUser { get; private set; }

        public LoginWindow(AuthService authService)
        {
            InitializeComponent();

            _authService = authService;
            Loaded += (_, _) => LoginTextBox.Focus();
        }

        private async void OnLoginClick(object sender, RoutedEventArgs e)
        {
            LoginButtonState(false);

            try
            {
                string login = LoginTextBox.Text.Trim();
                string password = PasswordInput.Password;

                var result = await _authService.LoginAsync(login, password);

                if (!result.Success)
                {
                    MessageBox.Show(result.Error ?? "Не удалось выполнить вход.",
                        "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);

                    if (result.Error?.Contains("пароль") == true)
                    {
                        PasswordInput.Clear();
                        PasswordInput.Focus();
                    }
                    else
                    {
                        LoginTextBox.Focus();
                        LoginTextBox.SelectAll();
                    }

                    return;
                }

                LoggedInUser = result.User;
                Close();
            }
            catch (Exception exception)
            {
                ExceptionHandler.Handle(exception, "Вход в систему");
            }
            finally
            {
                LoginButtonState(true);
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            LoggedInUser = null;
            Close();
        }

        private void LoginButtonState(bool isEnabled)
        {
            LoginTextBox.IsEnabled = isEnabled;
            PasswordInput.IsEnabled = isEnabled;
        }
    }
}
