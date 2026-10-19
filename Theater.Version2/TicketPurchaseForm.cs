using System;
using System.Globalization;
using System.Windows.Forms;
using Theater.Common;

namespace Theater.Version2
{
    public partial class TicketPurchaseForm : Form
    {
        private readonly int spectatorId;
        private readonly int playId;

        public TicketPurchaseForm(int spectatorId, int playId, string title, DateTime? date, decimal price)
        {
            this.spectatorId = spectatorId;
            this.playId = playId;
            InitializeComponent();
            detailsLabel.Text = title + Environment.NewLine +
                (date.HasValue ? date.Value.ToString("dd.MM.yyyy HH:mm") : "Дата не указана") + "    " +
                price.ToString("N2", CultureInfo.CurrentCulture) + " руб.";
        }

        private void BuyClicked(object sender, EventArgs e)
        {
            string seat = seatBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(seat))
            {
                MessageBox.Show("Укажите место.", "Покупка билета", MessageBoxButtons.OK, MessageBoxIcon.Information);
                seatBox.Focus();
                return;
            }
            try
            {
                Db.BuyTicket(spectatorId, playId, seat);
                MessageBox.Show("Билет сохранён в базе данных.", "Покупка билета", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Покупка билета", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
