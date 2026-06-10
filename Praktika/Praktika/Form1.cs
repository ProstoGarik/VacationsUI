using System;
using System.Data;
using System.Windows.Forms;

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
            TableSelectComboBox.Items.Add("Отпуска");
            TableSelectComboBox.Items.Add("Рабочие");
            TableSelectComboBox.Items.Add("Подразделения");
        }

        private void TableSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
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
                tableDataGridView.DataSource = viewModel.VacationsData;
                AddActionButtons();
                AutoResizeColumns();
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке данных.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void AddActionButtons()
        {
            // Проверка на дублирование таблиц
            if (tableDataGridView.Columns["DeleteButton"] != null)
                tableDataGridView.Columns.Remove("DeleteButton");
            if (tableDataGridView.Columns["EditButton"] != null)
                tableDataGridView.Columns.Remove("EditButton");

            // Создание кнопки "Удалить"
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn
            {
                Name = "DeleteButton",
                HeaderText = "",
                Text = "Удалить",
                UseColumnTextForButtonValue = true,
                Width = 85,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            // Создание кнопки "Изменить"
            DataGridViewButtonColumn editColumn = new DataGridViewButtonColumn
            {
                Name = "EditButton",
                HeaderText = "",
                Text = "Изменить",
                UseColumnTextForButtonValue = true,
                Width = 85,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            // Добавление кнопок как часть таблицы
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
                // TODO: Реализовать редактирование
                MessageBox.Show($"Редактирование строки {e.RowIndex + 1}");
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
    }
}