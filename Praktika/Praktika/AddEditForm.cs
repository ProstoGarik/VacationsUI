using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Praktika
{
    /// <summary>Форма добавления и редактирования записи.</summary>
    public partial class AddEditForm : Form
    {
        private ViewModel viewModel;
        private TableLayoutPanel tableLayoutPanel;
        private List<Control> inputControls = new List<Control>();
        private List<string> columnNames = new List<string>();
        private Dictionary<string, ForeignKeyInfo> foreignKeys = new Dictionary<string, ForeignKeyInfo>();
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
        }

        private void FillControlsWithEditValues()
        {
            for (int i = 0; i < inputControls.Count && i < columnNames.Count; i++)
            {
                string colName = columnNames[i];
                if (!EditValues.ContainsKey(colName)) continue;

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
        }

        private void GenerateFields()
        {
            tableLayoutPanel.Controls.Clear();
            DataTable table = viewModel.VacationsData;

            string primaryKeyName = table.PrimaryKey.Length > 0
                ? table.PrimaryKey[0].ColumnName
                : table.Columns[0].ColumnName;

            var visibleColumns = table.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName != primaryKeyName)
                .ToList();

            inputControls.Clear();
            columnNames.Clear();

            for (int i = 0; i < visibleColumns.Count; i++)
            {
                DataColumn column = visibleColumns[i];
                string colName = column.ColumnName;
                columnNames.Add(colName);

                Label label = new Label
                {
                    Text = colName,
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
                else
                {
                    TextBox textBox = new TextBox
                    {
                        Name = "txt" + colName,
                        Dock = DockStyle.Fill
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

                    // Для рабочих показываем специальность рядом с табельным номером
                    if (fk.ForeignTable == "Рабочие" && fk.ValueColumn == "Табельный номер")
                    {
                        string specialty = row["Специальность"]?.ToString();
                        display = $"{display} ({specialty})";
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

                var visibleColumns = table.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName != primaryKeyName)
                    .ToList();

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
                    else if (control is TextBox textBox)
                    {
                        string text = textBox.Text.Trim();
                        if (string.IsNullOrEmpty(text) && !visibleColumns[i].AllowDBNull)
                        {
                            MessageBox.Show($"Поле '{colName}' не может быть пустым.", "Ошибка",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        value = string.IsNullOrEmpty(text) ? DBNull.Value : Convert.ChangeType(text, visibleColumns[i].DataType);
                    }
                    else continue;

                    values.Add(colName, value);
                }

                bool success;
                if (IsEditMode)
                {
                    success = viewModel.UpdateRow(viewModel.CurrentTable, primaryKeyName, PrimaryKeyValue, values);
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
    }
}
