using System.Drawing;
using System.Windows.Forms;

namespace Theater.Common
{
    public static class UiStyle
    {
        public static readonly Color Ink = Color.FromArgb(35, 48, 55);
        public static readonly Color Muted = Color.FromArgb(102, 116, 123);
        public static readonly Color Accent = Color.FromArgb(0, 119, 105);
        public static readonly Color Line = Color.FromArgb(218, 226, 226);

        public static void Prepare(Form form, string title)
        {
            form.Text = title;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimumSize = new Size(900, 620);
            form.Font = new Font("Segoe UI", 9F);
            form.BackColor = Color.White;
            form.ForeColor = Ink;
        }

        public static Button Button(string text, bool primary = false)
        {
            Button button = new Button
            {
                Text = text, AutoSize = true, Height = 34,
                Padding = new Padding(12, 4, 12, 4), FlatStyle = FlatStyle.Flat,
                BackColor = primary ? Accent : Color.White,
                ForeColor = primary ? Color.White : Ink,
                UseVisualStyleBackColor = false, Margin = new Padding(0, 0, 8, 0)
            };
            button.FlatAppearance.BorderColor = primary ? Accent : Line;
            return button;
        }

        public static void PrepareGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill; grid.BackgroundColor = Color.White; grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Line; grid.RowHeadersVisible = false; grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false; grid.ReadOnly = true; grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 246, 245);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Ink;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersHeight = 38; grid.RowTemplate.Height = 32;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(218, 239, 235);
            grid.DefaultCellStyle.SelectionForeColor = Ink;
        }
    }
}
