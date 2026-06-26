using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Praktika
{
    /// <summary>Форма добавления и редактирования записи.</summary>
    public partial class AddEditForm : Form
    {
        private const string UsersTableName = "Users";
        private const string RolesTableName = "Roles";
        private const string LoginColumnName = "Login";
        private const string PasswordHashColumnName = "PasswordHash";
        private const string RoleIdColumnName = "RoleID";

        private ViewModel viewModel;
        private TableLayoutPanel tableLayoutPanel;
        private List<Control> inputControls = new List<Control>();
        private List<string> columnNames = new List<string>();
        private Dictionary<string, ForeignKeyInfo> foreignKeys = new Dictionary<string, ForeignKeyInfo>();
        private NullableDatePicker vacationStartDatePicker;
        private NullableDatePicker vacationEndDatePicker;
        public bool IsEditMode { get; set; } = false;
        public object PrimaryKeyValue { get; set; }
        public Dictionary<string, object> EditValues { get; set; }

        public AddEditForm()
        {
            InitializeComponent();
            InitializeDynamicPanel();
        }

        private void InitializeDynamicPanel()
        {
            tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                ColumnCount = 2,
                Padding = new Padding(10)
            };
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            this.Controls.Add(tableLayoutPanel);
        }

        public void SetViewModel(ViewModel vm)
        {
            viewModel = vm;
        }

        private void AddEditForm_Load(object sender, EventArgs e)
        {
            if (viewModel?.VacationsData == null)
            {
                MessageBox.Show("Нет данных для создания полей.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            ConfigureForeignKeys();
            GenerateFields();

            if (IsEditMode && EditValues != null)
            {
                FillControlsWithEditValues();
            }

            ConfigureVacationDateRange();
        }

        private void FillControlsWithEditValues()
        {
            for (int i = 0; i < inputControls.Count && i < columnNames.Count; i++)
            {
                string colName = columnNames[i];
                if (!EditValues.ContainsKey(colName)) continue;
                if (IsUserPasswordColumn(colName)) continue;

                object value = EditValues[colName];
                Control control = inputControls[i];

                if (control is TextBox textBox)
                {
                    textBox.Text = value?.ToString() ?? "";
                }
                else if (control is ComboBox comboBox)
                {
                    foreach (ComboItem item in comboBox.Items)
                    {
                        if (Equals(item.Value, value))
                        {
                            comboBox.SelectedItem = item;
                            break;
                        }
                    }
                    if (comboBox.SelectedItem == null && value != null)
                    {
                        comboBox.Text = value.ToString();
                    }
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Checked = value != DBNull.Value && Convert.ToBoolean(value);
                }
                else if (control is NullableDatePicker nullableDatePicker)
                {
                    nullableDatePicker.SelectedDate = value == null || value == DBNull.Value
                        ? null
                        : Convert.ToDateTime(value).Date;
                }
                else if (control is DateTimePicker datePicker)
                {
                    if (value == null || value == DBNull.Value)
                    {
                        datePicker.Value = DateTime.Today;
                    }
                    else
                    {
                        datePicker.Value = Convert.ToDateTime(value);
                    }
                }
            }
        }

        /// <summary>Настройка внешних ключей для выпадающих списков.</summary>
        private void ConfigureForeignKeys()
        {
            foreignKeys.Clear();
            string currentTable = viewModel.CurrentTable;

            if (currentTable == "Отпуска")
            {
                foreignKeys["Табельный номер"] = new ForeignKeyInfo
                {
                    ForeignTable = "Рабочие",
                    ValueColumn = "Табельный номер",
                    DisplayColumn = "Табельный номер"
                };
            }
            else if (currentTable == "Рабочие")
            {
                foreignKeys["ID Подразделения"] = new ForeignKeyInfo
                {
                    ForeignTable = "Подразделения",
                    ValueColumn = "ID",
                    DisplayColumn = "ФИО Руководителя"
                };
            }
            else if (currentTable == UsersTableName)
            {
                foreignKeys[RoleIdColumnName] = new ForeignKeyInfo
                {
                    ForeignTable = RolesTableName,
                    ValueColumn = "ID",
                    DisplayColumn = "RoleName"
                };
            }
        }

        private void GenerateFields()
        {
            tableLayoutPanel.Controls.Clear();
            DataTable table = viewModel.VacationsData;

            string primaryKeyName = table.PrimaryKey.Length > 0
                ? table.PrimaryKey[0].ColumnName
                : table.Columns[0].ColumnName;

            var visibleColumns = GetEditableColumns(table, primaryKeyName);

            inputControls.Clear();
            columnNames.Clear();
            vacationStartDatePicker = null;
            vacationEndDatePicker = null;

            for (int i = 0; i < visibleColumns.Count; i++)
            {
                DataColumn column = visibleColumns[i];
                string colName = column.ColumnName;
                columnNames.Add(colName);

                Label label = new Label
                {
                    Text = GetFieldLabel(colName),
                    AutoSize = true,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    Dock = DockStyle.Fill
                };

                Control inputControl;

                if (foreignKeys.ContainsKey(colName))
                {
                    var fk = foreignKeys[colName];
                    ComboBox comboBox = new ComboBox
                    {
                        Name = "cmb" + colName,
                        Dock = DockStyle.Fill,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    LoadComboBoxData(comboBox, fk);
                    inputControl = comboBox;
                }
                else if (column.DataType == typeof(bool))
                {
                    CheckBox checkBox = new CheckBox
                    {
                        Name = "chk" + colName,
                        Dock = DockStyle.Fill,
                        CheckAlign = System.Drawing.ContentAlignment.MiddleLeft
                    };
                    inputControl = checkBox;
                }
                else if (column.DataType == typeof(DateTime))
                {
                    if (viewModel?.CurrentTable == "Отпуска"
                        && (IsVacationStartDateColumn(colName) || IsVacationEndDateColumn(colName)))
                    {
                        NullableDatePicker datePicker = new NullableDatePicker
                        {
                            Name = "ndp" + colName,
                            Dock = DockStyle.Fill,
                            ShowToday = false
                        };
                        RegisterVacationDatePicker(colName, datePicker);
                        inputControl = datePicker;
                    }
                    else
                    {
                        DateTimePicker datePicker = new DateTimePicker
                        {
                            Name = "dtp" + colName,
                            Dock = DockStyle.Fill,
                            Format = DateTimePickerFormat.Short,
                            Value = DateTime.Today
                        };
                        inputControl = datePicker;
                    }
                }
                else
                {
                    TextBox textBox = new TextBox
                    {
                        Name = "txt" + colName,
                        Dock = DockStyle.Fill,
                        UseSystemPasswordChar = IsUserPasswordColumn(colName)
                    };
                    inputControl = textBox;
                }

                inputControls.Add(inputControl);
                tableLayoutPanel.Controls.Add(label, 0, i);
                tableLayoutPanel.Controls.Add(inputControl, 1, i);
            }

            Button btnSave = new Button { Text = "Сохранить", Dock = DockStyle.Fill };
            Button btnCancel = new Button { Text = "Отмена", Dock = DockStyle.Fill };
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, ev) => this.DialogResult = DialogResult.Cancel;

            int rowCount = visibleColumns.Count;
            tableLayoutPanel.RowCount = rowCount + 2;
            tableLayoutPanel.Controls.Add(btnSave, 0, rowCount);
            tableLayoutPanel.Controls.Add(btnCancel, 1, rowCount);
        }

        private void LoadComboBoxData(ComboBox comboBox, ForeignKeyInfo fk)
        {
            try
            {
                DataTable foreignData = viewModel.GetTableData(fk.ForeignTable);
                if (foreignData == null || foreignData.Rows.Count == 0)
                {
                    comboBox.Items.Add("Нет данных");
                    comboBox.Enabled = false;
                    return;
                }

                var items = new List<ComboItem>();
                foreach (DataRow row in foreignData.Rows)
                {
                    object value = row[fk.ValueColumn];
                    object display = row[fk.DisplayColumn];

                    if (fk.ForeignTable == "Рабочие" && fk.ValueColumn == "Табельный номер")
                    {
                        string tabNumber = row["Табельный номер"]?.ToString() ?? "";
                        string specialty = row["Специальность"]?.ToString() ?? "";
                        string departmentId = row["ID Подразделения"]?.ToString() ?? "";
                        display = $"{tabNumber} ({specialty} из {departmentId} подр.)";
                    }
                    items.Add(new ComboItem { Value = value, Display = display.ToString() });
                }
                comboBox.DataSource = items;
                comboBox.DisplayMember = "Display";
                comboBox.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBox.Items.Add("Ошибка");
                comboBox.Enabled = false;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable table = viewModel.VacationsData;
                string primaryKeyName = table.PrimaryKey.Length > 0
                    ? table.PrimaryKey[0].ColumnName
                    : table.Columns[0].ColumnName;

                var visibleColumns = GetEditableColumns(table, primaryKeyName);

                var values = new Dictionary<string, object>();

                for (int i = 0; i < visibleColumns.Count; i++)
                {
                    string colName = visibleColumns[i].ColumnName;
                    Control control = inputControls[i];
                    object value;

                    if (control is ComboBox comboBox)
                    {
                        if (comboBox.SelectedItem == null)
                        {
                            MessageBox.Show($"Выберите значение для поля '{colName}'.", "Ошибка",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        value = ((ComboItem)comboBox.SelectedItem).Value;
                    }
                    else if (control is CheckBox checkBox)
                    {
                        value = checkBox.Checked;
                    }
                    else if (control is NullableDatePicker nullableDatePicker)
                    {
                        if (!nullableDatePicker.SelectedDate.HasValue)
                        {
                            MessageBox.Show($"Выберите значение для поля '{GetFieldLabel(colName)}'.", "Ошибка",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        value = nullableDatePicker.SelectedDate.Value.Date;
                    }
                    else if (control is DateTimePicker datePicker)
                    {
                        value = datePicker.Value.Date;
                    }
                    else if (control is TextBox textBox)
                    {
                        string text = textBox.Text.Trim();
                        if (string.IsNullOrEmpty(text))
                        {
                            MessageBox.Show($"Поле '{colName}' не может быть пустым.", "Ошибка",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        value = Convert.ChangeType(text, visibleColumns[i].DataType);
                    }
                    else continue;

                    values.Add(colName, value);
                }

                if (!TryAddCalculatedVacationDuration(table, values))
                    return;

                DebugWriteFormValues(values);

                bool success;
                if (IsEditMode)
                {
                    success = viewModel.UpdateRow(viewModel.CurrentTable, primaryKeyName, PrimaryKeyValue, values);
                }
                else if (viewModel.CurrentTable == UsersTableName)
                {
                    success = CreateUser(values);
                }
                else
                {
                    success = viewModel.AddRow(values);
                }

                if (success)
                    this.DialogResult = DialogResult.OK;
                else
                    MessageBox.Show("Не удалось сохранить запись.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CreateUser(Dictionary<string, object> values)
        {
            string login = GetRequiredTextValue(values, LoginColumnName, "логин");
            string password = GetRequiredTextValue(values, PasswordHashColumnName, "пароль");

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                return false;

            if (!values.TryGetValue(RoleIdColumnName, out object roleValue) || roleValue == DBNull.Value)
            {
                MessageBox.Show("Выберите роль пользователя.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (viewModel.UserExists(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            int roleId = Convert.ToInt32(roleValue);
            viewModel.CreateUser(login, password, roleId);
            return true;
        }

        private static string GetRequiredTextValue(Dictionary<string, object> values, string columnName, string fieldName)
        {
            if (!values.TryGetValue(columnName, out object value) || value == DBNull.Value)
            {
                MessageBox.Show($"Введите {fieldName}.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return string.Empty;
            }

            string text = value?.ToString()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show($"Введите {fieldName}.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return text;
        }

        private bool IsUserPasswordColumn(string columnName)
        {
            return viewModel?.CurrentTable == UsersTableName && columnName == PasswordHashColumnName;
        }

        private List<DataColumn> GetEditableColumns(DataTable table, string primaryKeyName)
        {
            return table.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName != primaryKeyName)
                .Where(c => !IsCalculatedVacationDurationColumn(c.ColumnName))
                .ToList();
        }

        private bool IsCalculatedVacationDurationColumn(string columnName)
        {
            return viewModel?.CurrentTable == "Отпуска"
                && columnName.Equals("Продолжительность", StringComparison.OrdinalIgnoreCase);
        }

        private void RegisterVacationDatePicker(string columnName, NullableDatePicker datePicker)
        {
            if (viewModel?.CurrentTable != "Отпуска")
                return;

            if (IsVacationStartDateColumn(columnName))
                vacationStartDatePicker = datePicker;
            else if (IsVacationEndDateColumn(columnName))
                vacationEndDatePicker = datePicker;
        }

        private void ConfigureVacationDateRange()
        {
            if (vacationStartDatePicker == null || vacationEndDatePicker == null)
                return;

            void UpdateEndDateMinimum()
            {
                if (!vacationStartDatePicker.SelectedDate.HasValue)
                {
                    vacationEndDatePicker.SelectedDate = null;
                    vacationEndDatePicker.Enabled = false;
                    return;
                }

                DateTime minEndDate = vacationStartDatePicker.SelectedDate.Value.Date.AddDays(1);
                vacationEndDatePicker.Enabled = true;
                vacationEndDatePicker.MinDate = minEndDate;

                if (vacationEndDatePicker.SelectedDate.HasValue
                    && vacationEndDatePicker.SelectedDate.Value.Date < minEndDate)
                {
                    vacationEndDatePicker.SelectedDate = null;
                }
            }

            vacationStartDatePicker.SelectedDateChanged += (s, e) => UpdateEndDateMinimum();
            vacationEndDatePicker.CloseUp += (s, e) => EnsureEndDateIsAfterStart();
            vacationEndDatePicker.Validating += (s, e) => EnsureEndDateIsAfterStart();
            UpdateEndDateMinimum();
        }

        private void EnsureEndDateIsAfterStart()
        {
            if (vacationStartDatePicker == null || vacationEndDatePicker == null
                || !vacationStartDatePicker.SelectedDate.HasValue
                || !vacationEndDatePicker.SelectedDate.HasValue)
            {
                return;
            }

            DateTime minEndDate = vacationStartDatePicker.SelectedDate.Value.Date.AddDays(1);
            if (vacationEndDatePicker.SelectedDate.Value.Date < minEndDate)
                vacationEndDatePicker.SelectedDate = minEndDate;
        }

        private bool TryAddCalculatedVacationDuration(DataTable table, Dictionary<string, object> values)
        {
            if (viewModel?.CurrentTable != "Отпуска")
                return true;

            string durationColumnName = table.Columns.Cast<DataColumn>()
                .FirstOrDefault(c => IsCalculatedVacationDurationColumn(c.ColumnName))
                ?.ColumnName;

            if (string.IsNullOrEmpty(durationColumnName))
                return true;

            string startColumnName = table.Columns.Cast<DataColumn>()
                .FirstOrDefault(c => IsVacationStartDateColumn(c.ColumnName))
                ?.ColumnName;

            string endColumnName = table.Columns.Cast<DataColumn>()
                .FirstOrDefault(c => IsVacationEndDateColumn(c.ColumnName))
                ?.ColumnName;

            if (string.IsNullOrEmpty(startColumnName) || string.IsNullOrEmpty(endColumnName)
                || !values.TryGetValue(startColumnName, out object startValue)
                || !values.TryGetValue(endColumnName, out object endValue))
            {
                MessageBox.Show("Не удалось определить даты отпуска для расчета продолжительности.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DateTime startDate = Convert.ToDateTime(startValue).Date;
            DateTime endDate = Convert.ToDateTime(endValue).Date;
            if (endDate <= startDate)
            {
                MessageBox.Show("Дата окончания отпуска должна быть больше даты начала.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            values[durationColumnName] = (endDate - startDate).Days;
            return true;
        }

        private static bool IsVacationStartDateColumn(string columnName)
        {
            string normalized = columnName.ToLowerInvariant();
            return normalized.Contains("нач");
        }

        private static bool IsVacationEndDateColumn(string columnName)
        {
            string normalized = columnName.ToLowerInvariant();
            return normalized.Contains("окон") || normalized.Contains("конец");
        }

        private string GetFieldLabel(string columnName)
        {
            if (IsUserPasswordColumn(columnName))
                return "Пароль";

            if (viewModel?.CurrentTable == "Отпуска" && columnName == "Табельный номер")
                return "Рабочий";

            return columnName;
        }

        private void DebugWriteFormValues(Dictionary<string, object> values)
        {
            Debug.WriteLine($"[AddEditForm] Save requested. Table='{viewModel.CurrentTable}', IsEditMode='{IsEditMode}'.");
            foreach (var item in values)
            {
                object value = item.Value;
                string clrType = value == null || value == DBNull.Value ? "null" : value.GetType().FullName;
                string text = item.Key.Contains("Password", StringComparison.OrdinalIgnoreCase)
                    ? "<redacted>"
                    : value?.ToString() ?? "null";

                Debug.WriteLine($"[AddEditForm]   {item.Key}: ClrType='{clrType}', Value='{text}'");
            }
        }

        private class ForeignKeyInfo
        {
            public string ForeignTable { get; set; }
            public string ValueColumn { get; set; }
            public string DisplayColumn { get; set; }
        }

        private class ComboItem
        {
            public object Value { get; set; }
            public string Display { get; set; }
            public override string ToString() => Display;
        }

        private class NullableDatePicker : UserControl
        {
            private readonly TextBox textBox;
            private readonly Button button;
            private readonly MonthCalendar calendar;
            private readonly ToolStripDropDown dropDown;
            private DateTime? selectedDate;

            public event EventHandler SelectedDateChanged;
            public event EventHandler CloseUp;

            public NullableDatePicker()
            {
                textBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true
                };

                button = new Button
                {
                    Dock = DockStyle.Right,
                    Width = 28,
                    Text = string.Empty
                };

                calendar = new MonthCalendar
                {
                    MaxSelectionCount = 1,
                    ShowToday = false,
                    ShowTodayCircle = false
                };

                ToolStripControlHost host = new ToolStripControlHost(calendar)
                {
                    Margin = Padding.Empty,
                    Padding = Padding.Empty,
                    AutoSize = false,
                    Size = calendar.Size
                };

                dropDown = new ToolStripDropDown
                {
                    Padding = Padding.Empty
                };
                dropDown.Items.Add(host);

                Controls.Add(textBox);
                Controls.Add(button);

                Height = textBox.PreferredHeight;
                MinimumSize = new System.Drawing.Size(0, textBox.PreferredHeight);

                button.Click += (s, e) => OpenCalendar();
                button.Paint += Button_Paint;
                textBox.Click += (s, e) => OpenCalendar();
                calendar.DateSelected += Calendar_DateSelected;
                dropDown.Closed += (s, e) => CloseUp?.Invoke(this, EventArgs.Empty);
            }

            public DateTime? SelectedDate
            {
                get => selectedDate;
                set
                {
                    selectedDate = value?.Date;
                    textBox.Text = selectedDate.HasValue
                        ? selectedDate.Value.ToShortDateString()
                        : string.Empty;

                    if (selectedDate.HasValue)
                        calendar.SelectionStart = selectedDate.Value;

                    SelectedDateChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public DateTime MinDate
            {
                get => calendar.MinDate;
                set
                {
                    calendar.MinDate = value.Date;
                    if (selectedDate.HasValue && selectedDate.Value.Date < calendar.MinDate.Date)
                        SelectedDate = null;
                }
            }

            public bool ShowToday
            {
                get => calendar.ShowToday;
                set
                {
                    calendar.ShowToday = value;
                    calendar.ShowTodayCircle = value;
                }
            }

            private void OpenCalendar()
            {
                if (!Enabled)
                    return;

                DateTime initialDate = selectedDate ?? DateTime.Today;
                if (initialDate < calendar.MinDate)
                    initialDate = calendar.MinDate;

                calendar.SelectionStart = initialDate;
                calendar.SelectionEnd = initialDate;
                dropDown.Show(this, 0, Height);
            }

            private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
            {
                SelectedDate = e.Start.Date;
                dropDown.Close();
            }

            private void Button_Paint(object sender, PaintEventArgs e)
            {
                System.Drawing.Rectangle bounds = button.ClientRectangle;
                int iconSize = Math.Min(bounds.Width - 10, bounds.Height - 8);
                int x = bounds.Left + (bounds.Width - iconSize) / 2;
                int y = bounds.Top + (bounds.Height - iconSize) / 2;
                System.Drawing.Rectangle icon = new System.Drawing.Rectangle(x, y, iconSize, iconSize);

                using System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(45, 45, 45));
                using System.Drawing.Brush headerBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(70, 130, 180));

                e.Graphics.FillRectangle(System.Drawing.Brushes.White, icon);
                e.Graphics.DrawRectangle(pen, icon);
                e.Graphics.FillRectangle(headerBrush, icon.Left + 1, icon.Top + 1, icon.Width - 1, Math.Max(3, icon.Height / 4));

                int rowY = icon.Top + icon.Height / 2;
                e.Graphics.DrawLine(pen, icon.Left + 3, rowY, icon.Right - 3, rowY);
                e.Graphics.DrawLine(pen, icon.Left + icon.Width / 2, rowY - 2, icon.Left + icon.Width / 2, icon.Bottom - 3);
            }
        }
    }
}
