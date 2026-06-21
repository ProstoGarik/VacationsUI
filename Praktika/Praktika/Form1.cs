using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ClassLibrary;
using System.Linq;

namespace Praktika
{
    public partial class MainForm : Form
    {
        private ViewModel viewModel;

        public MainForm()
        {
            InitializeComponent();
            viewModel = new ViewModel();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Заполнение ComboBox для таблиц
            TableSelectComboBox.Items.Add("Отпуска");
            TableSelectComboBox.Items.Add("Рабочие");
            TableSelectComboBox.Items.Add("Подразделения");

            // Загружаем первую таблицу, чтобы установить путь к БД
            TableSelectComboBox.SelectedIndex = 0;
            RefreshTable();

            // Заполнение ComboBox для запросов
            quierySelectComboBox.Items.Add("Отпуска по специальности");
            quierySelectComboBox.SelectedIndex = 0; // автоматически вызовет событие

            // Подписываем события
            quierySelectComboBox.SelectedIndexChanged += quierySelectComboBox_SelectedIndexChanged;
            quieryParamComboBox.SelectedIndexChanged += quieryParamComboBox_SelectedIndexChanged;
        }

        private void TableSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void RefreshTable()
        {
            switch (TableSelectComboBox.SelectedIndex)
            {
                case 0:
                    LoadTable("Отпуска");
                    break;
                case 1:
                    LoadTable("Рабочие");
                    break;
                case 2:
                    LoadTable("Подразделения");
                    break;
            }
        }

        private void LoadTable(string table)
        {
            string dbPath = @"C:\Users\ediga\OneDrive\Документы\ОтпускаРабочихТолькоТаблицы.accdb";
            string dbPassword = string.Empty;

            viewModel.LoadTable(table, dbPath, dbPassword);

            if (viewModel.VacationsData != null)
            {
                BindTableData();
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке данных.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddActionButtons()
        {
            if (tableDataGridView.Columns["DeleteButton"] != null)
                tableDataGridView.Columns.Remove("DeleteButton");
            if (tableDataGridView.Columns["EditButton"] != null)
                tableDataGridView.Columns.Remove("EditButton");

            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn
            {
                Name = "DeleteButton",
                HeaderText = "",
                Text = "Удалить",
                UseColumnTextForButtonValue = true,
                Width = 85,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            DataGridViewButtonColumn editColumn = new DataGridViewButtonColumn
            {
                Name = "EditButton",
                HeaderText = "",
                Text = "Изменить",
                UseColumnTextForButtonValue = true,
                Width = 85,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            tableDataGridView.Columns.Add(deleteColumn);
            tableDataGridView.Columns.Add(editColumn);
        }

        private void AutoResizeColumns()
        {
            foreach (DataGridViewColumn col in tableDataGridView.Columns)
            {
                if (col.Name != "DeleteButton" && col.Name != "EditButton")
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        private void TableDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewColumn clickedColumn = tableDataGridView.Columns[e.ColumnIndex];
            DataGridViewRow row = tableDataGridView.Rows[e.RowIndex];
            DataRowView dataRow = row.DataBoundItem as DataRowView;
            if (dataRow == null) return;
            if (clickedColumn.Name == "DeleteButton")
            {
                DialogResult result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    bool deleted = DeleteRow(dataRow);
                    if (deleted)
                    {
                        MessageBox.Show("Запись удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (clickedColumn.Name == "EditButton")
            {
                DataTable dt = dataRow.DataView.Table;
                string pkColumn = dt.PrimaryKey.Length > 0
                    ? dt.PrimaryKey[0].ColumnName
                    : dt.Columns[0].ColumnName;
                object pkValue = dataRow[pkColumn];

                var editValues = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != pkColumn)
                        editValues[col.ColumnName] = dataRow[col.ColumnName];
                }

                using (AddEditForm editForm = new AddEditForm())
                {
                    editForm.SetViewModel(viewModel);
                    editForm.IsEditMode = true;
                    editForm.PrimaryKeyValue = pkValue;
                    editForm.EditValues = editValues;
                    if (editForm.ShowDialog(this) == DialogResult.OK)
                    {
                        RefreshTable();
                    }
                }
            }
        }

        private bool DeleteRow(DataRowView dataRow)
        {
            string currentTable = TableSelectComboBox.SelectedItem.ToString();
            DataTable dt = dataRow.DataView.Table;
            string pkColumn = dt.Columns[0].ColumnName;
            object pkValue = dataRow[pkColumn];

            bool deleted = viewModel.DeleteRow(currentTable, pkColumn, pkValue);
            return deleted;
        }

        private void AddRowButton_Click(object sender, EventArgs e)
        {
            if (viewModel.VacationsData == null)
            {
                MessageBox.Show("Сначала выберите таблицу.", "Предупреждение",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (AddEditForm addForm = new AddEditForm())
            {
                addForm.SetViewModel(viewModel);
                if (addForm.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshTable();
                }
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (viewModel.VacationsData == null)
            {
                MessageBox.Show("Сначала выберите таблицу.", "Предупреждение",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SortFilterForm filterForm = new SortFilterForm())
            {
                filterForm.SetViewModel(viewModel);
                if (filterForm.ShowDialog(this) == DialogResult.OK)
                {
                    BindTableData();
                }
            }
        }

        private void BindTableData()
        {
            tableDataGridView.DataSource = viewModel.VacationsData;
            AddActionButtons();
            AutoResizeColumns();
            UpdateStatusLabels();
        }

        private void UpdateStatusLabels()
        {
            FilterStatusLabel.Text = viewModel.IsFilterActive
                ? "Фильтр: Применяется"
                : "Фильтр: Отсутствует";
            SortStatusLabel.Text = viewModel.IsSortActive
                ? "Сортировка: Применяется"
                : "Сортировка: Отсутствует";
        }

        private void ResetFilterButton_Click(object sender, EventArgs e)
        {
            if (viewModel.VacationsData == null)
                return;

            viewModel.ClearFilter();
            BindTableData();
        }

        private void quierySelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = quierySelectComboBox.SelectedItem?.ToString();
            if (selected == "Отпуска по специальности")
            {
                LoadSpecialties();
            }
            else
            {
                // Для других запросов (пока нет) очищаем параметры и таблицу
                quieryParamComboBox.DataSource = null;
                quieryTableDataGridView.DataSource = null;
            }
        }

        private void LoadSpecialties()
        {
            // Получаем таблицу "Рабочие" через существующий метод ViewModel
            DataTable workers = viewModel.GetTableData("Рабочие");
            if (workers != null && workers.Rows.Count > 0)
            {
                // Извлекаем уникальные значения специальностей
                var specialties = workers.AsEnumerable()
                                         .Select(row => row.Field<string>("Специальность"))
                                         .Where(s => !string.IsNullOrEmpty(s))
                                         .Distinct()
                                         .ToList();
                quieryParamComboBox.DataSource = specialties;
                quieryParamComboBox.DisplayMember = "ToString";
                // Если есть хотя бы одна специальность, автоматически выбираем первую
                if (specialties.Count > 0)
                    quieryParamComboBox.SelectedIndex = 0;
            }
            else
            {
                quieryParamComboBox.DataSource = null;
                MessageBox.Show("Не удалось загрузить специальности.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void quieryParamComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSpecialty = quieryParamComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedSpecialty))
            {
                ExecuteVacationBySpecialtyQuery(selectedSpecialty);
            }
            else
            {
                quieryTableDataGridView.DataSource = null;
            }
        }

        private void ExecuteVacationBySpecialtyQuery(string specialty)
        {
            string query = @"
        SELECT Отпуска.* 
        FROM Отпуска 
        INNER JOIN Рабочие ON Отпуска.[Табельный номер] = Рабочие.[Табельный номер]
        WHERE Рабочие.Специальность = @spec";
            var parameters = new Dictionary<string, object> { { "spec", specialty } };
            DataTable result = viewModel.ExecuteCustomQuery(query, parameters);
            if (result != null)
            {
                quieryTableDataGridView.DataSource = result;
            }
            else
            {
                MessageBox.Show("Ошибка при выполнении запроса.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
