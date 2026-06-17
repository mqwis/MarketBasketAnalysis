using System.Windows;

namespace MarketBasketAnalysis
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim().ToLower();
            string password = TxtPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля для входа!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (login == "admin" && password == "admin")
            {
                OpenMainWindow("Администратор");
            }
            else if (login == "analyst" && password == "123")
            {
                OpenMainWindow("Аналитик");
            }
            else if (login == "manager" && password == "123")
            {
                OpenMainWindow("Менеджер");
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!\n\nТестовые входы:\n- admin / admin\n- analyst / 123\n- manager / 123",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenMainWindow(string userRole)
        {
            MessageBox.Show($"Успешный вход! Ваша роль: {userRole}", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

            MainWindow mainWin = new MainWindow(userRole);
            mainWin.Show();

            this.Close();
        }
    }
}