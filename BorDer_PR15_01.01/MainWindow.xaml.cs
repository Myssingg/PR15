using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BorDer_PR15_01._01
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Entities db = new Entities();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            Autorize autorize = new Autorize(db, this);
            autorize.Show();
            this.Hide();
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            if (!isEverythingFill())
            {
                MessageBox.Show("Заполнены не все поля!");
                return;
            }
            try
            {
                textBox_PhoneNumber.Text = getGoodNumber();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (!isEmailValid())
            {
                MessageBox.Show("Вы ввели неверный email адрес");
                return;
            }

            try
            {
                isPasswordsValid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            if (db.People.Where(x => x.Login == textBox_Login.Text).FirstOrDefault() != null)
            {
                MessageBox.Show("Пользователь с таким логином уже занят");
                return;
            }

            //Хэшируем пароль
            var hasher = new PasswordHasher<string>();
            string hashedPassword = hasher.HashPassword(null, textBox_Password.Text);


            //Запись в бд
            var entity = new Person();
            textBox_Surname.Text = (textBox_Surname.Text.Substring(0, 1).ToUpper()) + (textBox_Surname.Text.Substring(1).ToLower());
            textBox_Name.Text = (textBox_Name.Text.Substring(0, 1).ToUpper()) + (textBox_Name.Text.Substring(1).ToLower());
            
            entity.Surname = textBox_Surname.Text;
            entity.Name = textBox_Name.Text;
            if (!String.IsNullOrEmpty(textBox_Patronymic.Text))
            {
                textBox_Patronymic.Text = (textBox_Patronymic.Text.Substring(0, 1).ToUpper()) + (textBox_Patronymic.Text.Substring(1).ToLower());
                entity.Patronymic = textBox_Patronymic.Text;
            }

            entity.Login = textBox_Login.Text;
            entity.PhoneNumber = textBox_PhoneNumber.Text;
            entity.Email = textBox_Email.Text;
            entity.Password = hashedPassword;

            db.People.Add(entity);
            try
            {
                db.SaveChanges();
                MessageBox.Show("Пользователь успешно создан");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Debug.WriteLine(ex.ToString());
                return;
            }

            clearFields();
        }

        private bool isEverythingFill()
        {
            if (!String.IsNullOrEmpty(textBox_Surname.Text) &&
                !String.IsNullOrEmpty(textBox_Name.Text) &&
                !String.IsNullOrEmpty(textBox_Login.Text) &&
                !String.IsNullOrEmpty(textBox_PhoneNumber.Text) &&
                !String.IsNullOrEmpty(textBox_Email.Text) &&
                !String.IsNullOrEmpty(textBox_Password.Text) &&
                !String.IsNullOrEmpty(textBox_ConfirmPassword.Text))
                return true;
            return false;
        }

        private string getGoodNumber()
        {
            Regex regexNumber = new Regex(@"\D");
            string phoneNumber = regexNumber.Replace(textBox_PhoneNumber.Text, "");

            //Заменяем первую 8 на 7
            if (phoneNumber[0] == '8')
            {
                phoneNumber = phoneNumber.Remove(0, 1);
                phoneNumber = phoneNumber.Insert(0, "7");
            }
            else if (phoneNumber[0] != '7')
            {
                throw new Exception("Номер телефона должен начинаться с 7 или 8!");
            }

            if (phoneNumber.Length != 11)
            {
                throw new Exception("Номер телефона не имеет 11 цифр");
            }

            return phoneNumber;
        }

        private bool isEmailValid()
        {
            Regex regexEmail = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$");
            string email = textBox_Email.Text.Trim();
            if (regexEmail.IsMatch(email)) return true;
            else return false;
        }

        private bool isPasswordsValid()
        {
            string password = textBox_Password.Text.Trim();
            string confirmPassword = textBox_ConfirmPassword.Text.Trim();

            Regex regexPassword = new Regex(@"\s+"); //Если пробел повторяется 1 и более раз
            if (regexPassword.IsMatch(password))
            {
                throw new Exception("В пароле содержатся пробелы!");
            }

            Regex regexZagl = new Regex(@"[A-Z]+");
            Regex regexStrochn = new Regex(@"[a-z]+");

            if (!regexZagl.IsMatch(password) || !regexStrochn.IsMatch(password) || !isOneNumberInString(password))
            {
                throw new Exception("Пароль не содержит одновременно одну строчную, одну заглавную буквы и одну цифру!");
                
            }

            if (password.Length < 8)
                throw new Exception("Пароль должен быть не менее 8 символов!");

            if (password != confirmPassword)
                throw new Exception("Пароли не совпадают!");

            return true;
        }

        private bool isOneNumberInString(string password)
        {
            for (int i = 0; i < password.Length; i++)
            {
                if (Char.IsDigit(password[i])) return true;
            }
            return false;
        }

        private void clearFields()
        {
            textBox_Surname.Text = String.Empty;
            textBox_Name.Text = String.Empty;
            textBox_Patronymic.Text = String.Empty;
            textBox_Login.Text = String.Empty;
            textBox_PhoneNumber.Text = String.Empty;
            textBox_Email.Text = String.Empty;
            textBox_Password.Text = String.Empty;
            textBox_ConfirmPassword.Text = String.Empty;
        }
    }
}
