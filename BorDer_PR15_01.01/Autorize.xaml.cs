using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

namespace BorDer_PR15_01._01
{
    /// <summary>
    /// Логика взаимодействия для Autorize.xaml
    /// </summary>
    public partial class Autorize : Window
    {
        Entities db;
        MainWindow reg;
        public Autorize(Entities db, MainWindow reg)
        {
            InitializeComponent();
            this.db = db;
            this.reg = reg;
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            var person = db.People.Where(x => x.Login == textBox_Login.Text).FirstOrDefault();
            if (person == null)
            {
                MessageBox.Show("Пользователь с таким логином не найден!");
                return;
            }

            string password = person.Password;

            var hasher = new PasswordHasher<string>();

            var result = hasher.VerifyHashedPassword(null, password, textBox_Password.Text);

            if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                MessageBox.Show("Успешный вход", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                clearFields();
            }
            else
            {
                MessageBox.Show("Пароль неверный", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clearFields()
        {
            textBox_Login.Text = string.Empty;
            textBox_Password.Text = string.Empty;
        }
    }
}
