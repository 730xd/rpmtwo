using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
using rpmtwo.Models;

namespace rpmtwo
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AeroflotDbContext db;
        private string currentCaptcha;
        private Random random = new Random();

        
        public LoginWindow()
        {
            InitializeComponent();
            db = new AeroflotDbContext();
            GenerateCaptcha();
        }
        private void GenerateCaptcha()
        {
            int captchaNumber = random.Next(1000, 9999);
            currentCaptcha = captchaNumber.ToString();
            lblCaptcha.Text = currentCaptcha;
        }

        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            txtCaptcha.Clear();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = txtLogin.Text.Trim();
                string password = txtPassword.Password;
                string captcha = txtCaptcha.Text.Trim();

                // Проверка капчи
                if (!captcha.Equals(currentCaptcha))
                {
                    //tbStatus.Text = "Неверный код с картинки!";
                    GenerateCaptcha();
                    txtCaptcha.Clear();
                    return;
                }

                // Проверка на пустые поля
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    //tbStatus.Text = "Введите логин и пароль!";
                    return;
                }

                // Поиск пользователя в БД
                var user = db.Users.FirstOrDefault(u => u.UserLogin == login && u.UserPassword == password);

                if (user == null)
                {
                    //tbStatus.Text = "Неверный логин или пароль!";
                    GenerateCaptcha();
                    txtCaptcha.Clear();
                    return;
                }

                // Получаем роль пользователя
                var role = db.Roles.FirstOrDefault(r => r.RoleId == user.UserRole);

                MessageBox.Show($"Добро пожаловать, {user.UserName} {user.UserPatronymic}!\n" +
                                $"Ваша роль: {role?.RoleName}", "Успешно",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                // ПРЯМОЕ УСЛОВИЕ - если логин и пароль верные, открываем MainWindow
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();

                // Закрываем окно логина
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка");
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}


