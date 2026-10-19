using System;
using System.Windows.Forms;

namespace Theater.Common
{
    public partial class LoginForm : Form
    {
        public AuthUser User { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(loginBox.Text) || string.IsNullOrEmpty(passwordBox.Text))
            {
                MessageBox.Show("Введите логин и пароль.", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                User = Db.Authenticate(loginBox.Text, passwordBox.Text);
                if (User == null)
                {
                    MessageBox.Show("Неверный логин или пароль.", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    passwordBox.Clear();
                    passwordBox.Focus();
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegisterClicked(object sender, EventArgs e)
        {
            using (ViewerRegistrationForm registration = new ViewerRegistrationForm())
            {
                if (registration.ShowDialog(this) != DialogResult.OK) return;
                User = registration.User;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
