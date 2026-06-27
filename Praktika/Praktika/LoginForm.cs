using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;

namespace Praktika
{
    public partial class LoginForm : Form
    {
        private readonly ViewModel viewModel;

        public AuthenticatedUser? CurrentUser { get; private set; }

        public LoginForm(string dbPath, string dbPassword)
        {
            InitializeComponent();
            viewModel = new ViewModel();
            viewModel.ConfigureAuthDatabase(dbPath, dbPassword);

            Text = "Вход";
            PasswordTextBox.UseSystemPasswordChar = true;
            AcceptButton = LoginButton;
            LoginButton.Click += LoginButton_Click;
        }

        private void LoginButton_Click(object? sender, EventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Вход",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                CurrentUser = viewModel.AuthenticateUser(login, password);
                if (CurrentUser == null)
                {
                    MessageBox.Show("Неверный логин или пароль.", "Вход",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
