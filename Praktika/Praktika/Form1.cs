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
            string dbPath = @"C:\Users\ediga\OneDrive\Документы\ОтпускаРабочихТолькоТаблицы.accdb";
            string dbPassword = string.Empty; // Возможное использование пароля

            viewModel.LoadData(dbPath, dbPassword);

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