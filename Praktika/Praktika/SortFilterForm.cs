using System;
using System.Windows.Forms;

namespace Praktika
{
    /// <summary>Форма задания фильтра по столбцу таблицы.</summary>
    public partial class SortFilterForm : Form
    {
        private ViewModel viewModel;

        public SortFilterForm()
        {
            InitializeComponent();
            Text = "Фильтр";
            ApplyFilterButton.Text = "Применить";
            CancelButton.Text = "Отмена";
            ApplyFilterButton.Click += ApplyFilterButton_Click;
            CancelButton.Click += (s, e) => DialogResult = DialogResult.Cancel;
            Load += FilterForm_Load;
        }

        public void SetViewModel(ViewModel vm)
        {
            viewModel = vm;
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
            if (viewModel?.VacationsData == null)
            {
                MessageBox.Show("Нет данных для фильтрации.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            FilteredColumnComboBox.Items.Clear();
            foreach (string column in viewModel.GetColumnNames())
                FilteredColumnComboBox.Items.Add(column);

            if (FilteredColumnComboBox.Items.Count > 0)
                FilteredColumnComboBox.SelectedIndex = 0;

            FilterOperationComboBox.Items.Clear();
            FilterOperationComboBox.Items.AddRange(new object[] { ">", "<", "=", ">=", "<=", "!=" });
            FilterOperationComboBox.SelectedIndex = 2;
        }

        private void ApplyFilterButton_Click(object sender, EventArgs e)
        {
            if (FilteredColumnComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите столбец.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (FilterOperationComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите операцию.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string value = FilterValueTextBox.Text.Trim();
            if (string.IsNullOrEmpty(value))
            {
                MessageBox.Show("Введите значение для фильтра.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string column = FilteredColumnComboBox.SelectedItem.ToString();
            string operation = FilterOperationComboBox.SelectedItem.ToString();

            try
            {
                if (viewModel.ApplyFilter(column, operation, value))
                    DialogResult = DialogResult.OK;
                else
                    MessageBox.Show("Не удалось применить фильтр.", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат значения для выбранного столбца.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
