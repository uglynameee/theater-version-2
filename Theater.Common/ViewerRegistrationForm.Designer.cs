namespace Theater.Common
{
    partial class ViewerRegistrationForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label loginLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label confirmLabel;
        private System.Windows.Forms.Label firstNameLabel;
        private System.Windows.Forms.Label lastNameLabel;
        private System.Windows.Forms.Label phoneLabel;
        private System.Windows.Forms.TextBox loginBox;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.TextBox confirmBox;
        private System.Windows.Forms.TextBox firstNameBox;
        private System.Windows.Forms.TextBox lastNameBox;
        private System.Windows.Forms.TextBox phoneBox;
        private System.Windows.Forms.Button registerButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.loginLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.confirmLabel = new System.Windows.Forms.Label();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.phoneLabel = new System.Windows.Forms.Label();
            this.loginBox = new System.Windows.Forms.TextBox();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.confirmBox = new System.Windows.Forms.TextBox();
            this.firstNameBox = new System.Windows.Forms.TextBox();
            this.lastNameBox = new System.Windows.Forms.TextBox();
            this.phoneBox = new System.Windows.Forms.TextBox();
            this.registerButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // title
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.Location = new System.Drawing.Point(26, 20);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(204, 30);
            this.titleLabel.Text = "Регистрация зрителя";
            // labels
            this.loginLabel.AutoSize = true; this.loginLabel.Location = new System.Drawing.Point(30, 68); this.loginLabel.Name = "loginLabel"; this.loginLabel.Text = "Логин";
            this.passwordLabel.AutoSize = true; this.passwordLabel.Location = new System.Drawing.Point(30, 122); this.passwordLabel.Name = "passwordLabel"; this.passwordLabel.Text = "Пароль";
            this.confirmLabel.AutoSize = true; this.confirmLabel.Location = new System.Drawing.Point(30, 176); this.confirmLabel.Name = "confirmLabel"; this.confirmLabel.Text = "Повторите пароль";
            this.firstNameLabel.AutoSize = true; this.firstNameLabel.Location = new System.Drawing.Point(30, 230); this.firstNameLabel.Name = "firstNameLabel"; this.firstNameLabel.Text = "Имя";
            this.lastNameLabel.AutoSize = true; this.lastNameLabel.Location = new System.Drawing.Point(30, 284); this.lastNameLabel.Name = "lastNameLabel"; this.lastNameLabel.Text = "Фамилия";
            this.phoneLabel.AutoSize = true; this.phoneLabel.Location = new System.Drawing.Point(30, 338); this.phoneLabel.Name = "phoneLabel"; this.phoneLabel.Text = "Телефон (необязательно)";
            // inputs
            this.loginBox.Location = new System.Drawing.Point(30, 88); this.loginBox.Name = "loginBox"; this.loginBox.Size = new System.Drawing.Size(300, 23);
            this.passwordBox.Location = new System.Drawing.Point(30, 142); this.passwordBox.Name = "passwordBox"; this.passwordBox.PasswordChar = '*'; this.passwordBox.Size = new System.Drawing.Size(300, 23);
            this.confirmBox.Location = new System.Drawing.Point(30, 196); this.confirmBox.Name = "confirmBox"; this.confirmBox.PasswordChar = '*'; this.confirmBox.Size = new System.Drawing.Size(300, 23);
            this.firstNameBox.Location = new System.Drawing.Point(30, 250); this.firstNameBox.Name = "firstNameBox"; this.firstNameBox.Size = new System.Drawing.Size(300, 23);
            this.lastNameBox.Location = new System.Drawing.Point(30, 304); this.lastNameBox.Name = "lastNameBox"; this.lastNameBox.Size = new System.Drawing.Size(300, 23);
            this.phoneBox.Location = new System.Drawing.Point(30, 358); this.phoneBox.Name = "phoneBox"; this.phoneBox.Size = new System.Drawing.Size(300, 23);
            // button
            this.registerButton.Location = new System.Drawing.Point(30, 400);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(150, 36);
            this.registerButton.Text = "Создать аккаунт";
            this.registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.RegisterClicked);
            // form
            this.AcceptButton = this.registerButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 458);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.loginLabel); this.Controls.Add(this.passwordLabel); this.Controls.Add(this.confirmLabel);
            this.Controls.Add(this.firstNameLabel); this.Controls.Add(this.lastNameLabel); this.Controls.Add(this.phoneLabel);
            this.Controls.Add(this.loginBox); this.Controls.Add(this.passwordBox); this.Controls.Add(this.confirmBox);
            this.Controls.Add(this.firstNameBox); this.Controls.Add(this.lastNameBox); this.Controls.Add(this.phoneBox);
            this.Controls.Add(this.registerButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.MinimizeBox = false;
            this.Name = "ViewerRegistrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Регистрация зрителя";
            this.ResumeLayout(false); this.PerformLayout();
        }
    }
}
