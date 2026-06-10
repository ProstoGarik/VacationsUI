using System;
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
            string dbPassword = string.Empty; // Возможное использование пароля

            viewModel.LoadTable(table, dbPath, dbPassword);

            if (viewModel.VacationsData != null)
            {
                tableDataGridView.DataSource = viewModel.VacationsData;
                tableDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            else
            {
                MessageBox.Show("Ошибка при загрузке данных.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}