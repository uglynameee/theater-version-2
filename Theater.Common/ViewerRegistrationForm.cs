using System;
using System.Windows.Forms;

namespace Theater.Common
{
    public partial class ViewerRegistrationForm : Form
    {
        public AuthUser User { get; private set; }

        public ViewerRegistrationForm()
        {
            InitializeComponent();
        }

        private void RegisterClicked(object sender, EventArgs e)
        {
            string login = loginBox.Text.Trim();
            string firstName = firstNameBox.Text.Trim();
            string lastName = lastNameBox.Text.Trim();
            string password = passwordBox.Text;
            if (login.Length < 3 || password.Length < 4 || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Заполните имя и фамилию. Логин должен быть не короче 3 символов, пароль — 4.", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (password != confirmBox.Text)
            {
                MessageBox.Show("Пароли не совпадают.", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                confirmBox.Focus();
                return;
            }
            try
            {
                User = Db.RegisterViewer(login, password, firstName, lastName, phoneBox.Text.Trim());
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
