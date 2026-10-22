using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Theater.Common;

namespace Theater.Version2
{
    public partial class FullForm : Form
    {
        private DataTable data; private TableDefinition current;
        private readonly AuthUser user;

        public FullForm() : this(null) { }

        public FullForm(AuthUser user)
        {
            this.user = user;
            InitializeComponent();
        }

        private void FullForm_Load(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            UiStyle.PrepareGrid(grid);
            if (user != null && !user.IsEmployee)
            {
                tabControl.TabPages.Clear();
                tabControl.TabPages.Add(playsTab);
                tabControl.TabPages.Add(myTicketsTab);
                addButton.Visible = false;
                editButton.Visible = false;
                deleteButton.Visible = false;
                buyButton.Visible = true;
                status.Text = "Режим зрителя";
            }
            TableChanged(this, EventArgs.Empty);
        }

        private void Reload()
        {
            if (current == null) return;
            try
            {
                data = current.Name == "Билеты"
                    ? (user != null && !user.IsEmployee && user.SpectatorId.HasValue ? Db.ReadSpectatorTickets(user.SpectatorId.Value) : Db.ReadTickets())
                    : Db.ReadTable(current);
                ApplySearch();
            }
            catch (Exception ex) { Error(ex); }
        }

        private void ApplySearch()
        {
            if (data == null) return;
            string query = searchBox.Text.Trim().Replace("'", "''").Replace("%", "[%]").Replace("*", "[*]");
            data.DefaultView.RowFilter = "";
            if (query.Length > 0)
            {
                List<string> filters = new List<string>();
                foreach (DataColumn column in data.Columns) if (column.DataType == typeof(string)) filters.Add("[" + column.ColumnName + "] LIKE '*" + query + "*'");
                if (filters.Count > 0) data.DefaultView.RowFilter = string.Join(" OR ", filters);
            }
            grid.DataSource = data.DefaultView; TranslateHeaders(); status.Text = data.DefaultView.Count + " из " + data.Rows.Count + " записей | " + Db.DatabasePath;
        }

        private void TranslateHeaders()
        {
            if (grid.Columns.Contains("Код")) grid.Columns["Код"].HeaderText = "№";
            foreach (FieldDefinition field in current.Fields) if (grid.Columns.Contains(field.Name)) grid.Columns[field.Name].HeaderText = field.Label;
            if (current.Name == "Билеты")
            {
                grid.Columns["КодЗрителя"].Visible = false;
                grid.Columns["КодСпектакля"].Visible = false;
                grid.Columns["Покупатель"].HeaderText = "Зритель";
                grid.Columns["НазваниеСпектакля"].HeaderText = "Спектакль";
            }
            foreach (string name in new[] { "ДатаВремя", "ДатаПокупки", "ДатаОценки" }) if (grid.Columns.Contains(name)) grid.Columns[name].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            if (grid.Columns.Contains("ЦенаБилета")) grid.Columns["ЦенаБилета"].DefaultCellStyle.Format = "N2";
        }

        private void TableChanged(object sender, EventArgs e)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            int index = tabControl.SelectedTab == null ? -1 : Convert.ToInt32(tabControl.SelectedTab.Tag);
            if (index < 0 || index >= Db.Tables.Length) return;
            current = Db.Tables[index];
            grid.Parent = tabControl.SelectedTab;
            grid.Dock = DockStyle.Fill;
            grid.BringToFront();
            Reload();
        }
        private void RefreshClicked(object sender, EventArgs e) { Reload(); }
        private void BuyClicked(object sender, EventArgs e)
        {
            if (user == null || user.SpectatorId == null)
            {
                MessageBox.Show("Для покупки войдите в зарегистрированный аккаунт зрителя.", "Покупка билета", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (grid.CurrentRow == null || current == null || current.Name != "Спектакли")
            {
                MessageBox.Show("Сначала выберите спектакль в афише.", "Покупка билета", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int playId = Convert.ToInt32(grid.CurrentRow.Cells["Код"].Value);
            string title = Convert.ToString(grid.CurrentRow.Cells["Название"].Value);
            DateTime? date = grid.CurrentRow.Cells["ДатаВремя"].Value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(grid.CurrentRow.Cells["ДатаВремя"].Value);
            decimal price = grid.CurrentRow.Cells["ЦенаБилета"].Value == DBNull.Value ? 0 : Convert.ToDecimal(grid.CurrentRow.Cells["ЦенаБилета"].Value);
            using (TicketPurchaseForm purchase = new TicketPurchaseForm(user.SpectatorId.Value, playId, title, date, price))
                if (purchase.ShowDialog(this) == DialogResult.OK) Reload();
        }
        private void SearchChanged(object sender, EventArgs e) { ApplySearch(); }
        private void AddClicked(object sender, EventArgs e) { EditRecord(null); }
        private void EditClicked(object sender, EventArgs e) { EditRecord(grid.CurrentRow); }
        private void DoubleClicked(object sender, DataGridViewCellEventArgs e) { if (user != null && !user.IsEmployee) BuyClicked(sender, EventArgs.Empty); else EditRecord(grid.CurrentRow); }

        private void EditRecord(DataGridViewRow selected)
        {
            DataRowView view = selected == null ? null : selected.DataBoundItem as DataRowView;
            try
            {
                using (RecordEditor editor = new RecordEditor(current, view == null ? null : view.Row))
                    if (editor.ShowDialog(this) == DialogResult.OK) { if (view == null) Db.Add(current, editor.Values); else Db.Update(current, Convert.ToInt32(view["Код"]), editor.Values); Reload(); }
            }
            catch (Exception ex) { Error(ex); }
        }

        private void DeleteClicked(object sender, EventArgs e)
        {
            DataRowView view = grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as DataRowView;
            if (view == null) return;
            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { Db.Delete(current, Convert.ToInt32(view["Код"])); Reload(); } catch (Exception ex) { Error(ex); }
        }

        private void Error(Exception ex) { MessageBox.Show(ex.Message, "Ошибка базы данных", MessageBoxButtons.OK, MessageBoxIcon.Error); }

    }

    internal sealed class RecordEditor : Form
    {
        private readonly TableDefinition table; private readonly DataRow row; private readonly Dictionary<string, Control> inputs = new Dictionary<string, Control>();
        public Dictionary<string, object> Values { get; private set; }
        public RecordEditor(TableDefinition table, DataRow row)
        {
            this.table = table; this.row = row; UiStyle.Prepare(this, (row == null ? "Добавить: " : "Изменить: ") + table.Label); Width = 560; MinimumSize = new System.Drawing.Size(480, 300); AutoSize = true; AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 2, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink }; layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64)); Controls.Add(layout);
            foreach (FieldDefinition field in table.Fields) { Label label = new Label { Text = field.Label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 12, 8) }; Control input = CreateInput(field); input.Dock = DockStyle.Fill; input.Margin = new Padding(0, 4, 0, 4); inputs[field.Name] = input; layout.Controls.Add(label); layout.Controls.Add(input); }
            FlowLayoutPanel buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 10, 0, 0) }; Button ok = UiStyle.Button("Сохранить", true); Button cancel = UiStyle.Button("Отмена"); ok.Click += Save; cancel.DialogResult = DialogResult.Cancel; buttons.Controls.Add(ok); buttons.Controls.Add(cancel); layout.Controls.Add(buttons); layout.SetColumnSpan(buttons, 2); AcceptButton = ok; CancelButton = cancel; if (row != null) LoadValues();
        }

        private Control CreateInput(FieldDefinition field)
        {
            if (field.Kind == "fk") { ComboBox combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "НазваниеДляСписка", ValueMember = "Код" }; DataTable lookup = Lookup(field); DataRow blank = lookup.NewRow(); blank["Код"] = DBNull.Value; blank["НазваниеДляСписка"] = ""; lookup.Rows.InsertAt(blank, 0); combo.DataSource = lookup; return combo; }
            if (field.Kind == "date") return new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm", ShowUpDown = true, Value = DateTime.Now };
            if (field.Kind == "memo") return new TextBox { Multiline = true, Height = 72, ScrollBars = ScrollBars.Vertical };
            return new TextBox();
        }

        private DataTable Lookup(FieldDefinition field)
        {
            string sql;
            if (field.LookupTable == "Актёры" || field.LookupTable == "Режиссёры" || field.LookupTable == "Зрители") sql = "SELECT [Код], [Имя] & ' ' & [Фамилия] AS [НазваниеДляСписка] FROM [" + field.LookupTable + "] ORDER BY [Фамилия], [Имя]";
            else sql = "SELECT [Код], [" + field.LookupLabel + "] AS [НазваниеДляСписка] FROM [" + field.LookupTable + "] ORDER BY [" + field.LookupLabel + "]";
            return Db.Read(sql);
        }

        private void LoadValues()
        {
            foreach (FieldDefinition field in table.Fields) { object value = row[field.Name]; if (value == DBNull.Value) continue; Control input = inputs[field.Name]; if (input is ComboBox) ((ComboBox)input).SelectedValue = value; else if (input is DateTimePicker) ((DateTimePicker)input).Value = Convert.ToDateTime(value); else input.Text = Convert.ToString(value, CultureInfo.CurrentCulture); }
        }

        private void Save(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                foreach (FieldDefinition field in table.Fields)
                {
                    Control input = inputs[field.Name]; object value;
                    if (field.Kind == "fk") { object selected = ((ComboBox)input).SelectedValue; value = selected == null || selected == DBNull.Value ? (object)DBNull.Value : Convert.ToInt32(selected); }
                    else if (field.Kind == "date") value = ((DateTimePicker)input).Value;
                    else if (field.Kind == "int") { int number; if (!int.TryParse(input.Text, out number)) throw new InvalidOperationException("Поле «" + field.Label + "» должно содержать целое число."); if (field.Name == "Оценка" && (number < 1 || number > 5)) throw new InvalidOperationException("Оценка должна быть от 1 до 5."); value = number; }
                    else if (field.Kind == "money") { decimal money; if (!decimal.TryParse(input.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out money) || money < 0) throw new InvalidOperationException("Введите корректную неотрицательную цену."); value = money; }
                    else value = string.IsNullOrWhiteSpace(input.Text) ? (object)DBNull.Value : input.Text.Trim();
                    values[field.Name] = value;
                }
                Values = values; DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Проверьте данные", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}
