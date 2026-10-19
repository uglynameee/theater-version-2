namespace Theater.Version2
{
    partial class TicketPurchaseForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label headingLabel;
        private System.Windows.Forms.Label detailsLabel;
        private System.Windows.Forms.Label seatLabel;
        private System.Windows.Forms.TextBox seatBox;
        private System.Windows.Forms.Button buyButton;
        private System.Windows.Forms.Button cancelButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headingLabel = new System.Windows.Forms.Label();
            this.detailsLabel = new System.Windows.Forms.Label();
            this.seatLabel = new System.Windows.Forms.Label();
            this.seatBox = new System.Windows.Forms.TextBox();
            this.buyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // heading
            this.headingLabel.AutoSize = true;
            this.headingLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.headingLabel.Location = new System.Drawing.Point(24, 20);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Size = new System.Drawing.Size(188, 30);
            this.headingLabel.Text = "Покупка билета";
            // details
            this.detailsLabel.AutoSize = true;
            this.detailsLabel.Location = new System.Drawing.Point(28, 68);
            this.detailsLabel.MaximumSize = new System.Drawing.Size(390, 0);
            this.detailsLabel.Name = "detailsLabel";
            this.detailsLabel.Size = new System.Drawing.Size(0, 30);
            // seat
            this.seatLabel.AutoSize = true;
            this.seatLabel.Location = new System.Drawing.Point(28, 133);
            this.seatLabel.Name = "seatLabel";
            this.seatLabel.Size = new System.Drawing.Size(38, 15);
            this.seatLabel.Text = "Место";
            this.seatBox.Location = new System.Drawing.Point(28, 153);
            this.seatBox.Name = "seatBox";
            this.seatBox.Size = new System.Drawing.Size(250, 23);
            // buttons
            this.buyButton.Location = new System.Drawing.Point(28, 199);
            this.buyButton.Name = "buyButton";
            this.buyButton.Size = new System.Drawing.Size(142, 36);
            this.buyButton.Text = "Подтвердить";
            this.buyButton.UseVisualStyleBackColor = true;
            this.buyButton.Click += new System.EventHandler(this.BuyClicked);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(183, 199);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(95, 36);
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            // form
            this.AcceptButton = this.buyButton;
            this.CancelButton = this.cancelButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 263);
            this.Controls.Add(this.headingLabel);
            this.Controls.Add(this.detailsLabel);
            this.Controls.Add(this.seatLabel);
            this.Controls.Add(this.seatBox);
            this.Controls.Add(this.buyButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TicketPurchaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Покупка билета";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
