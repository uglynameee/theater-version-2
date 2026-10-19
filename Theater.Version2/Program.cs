using System;
using System.Windows.Forms;
using Theater.Common;

namespace Theater.Version2
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Db.Initialize();
                using (LoginForm login = new LoginForm())
                    if (login.ShowDialog() == DialogResult.OK) Application.Run(new FullForm(login.User));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
