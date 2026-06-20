using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ClassLibrary;

namespace Praktika
{
    /// <summary>Форма задания фильтра и сортировки.</summary>
    public partial class SortFilterForm : Form
    {
        private ViewModel viewModel;
        private bool isRestoringState;

        public SortFilterForm()
        {
            InitializeComponent();
            Text = "Фильтр и сортировка";
            ApplyFilterButton.Text = "Применить";
            CancelButton.Text = "Отмена";
            ApplyFilterButton.Click += ApplyButton_Click;
            CancelButton.Click += (s, e) => DialogResult = DialogResult.Cancel;
            ResetFilterButton.Click += ResetFilterButton_Click;
            ResetSortButton.Click += ResetSortButton_Click;
            Load += SortFilterForm_Load;
            FormClosing += SortFilterForm_FormClosing;
        }

        public void SetViewModel(ViewModel vm)
        {
            viewModel = vm;
        }

        private void SortFilterForm_Load(object sender, EventArgs e)
        {
            if (viewModel?.VacationsData == null)
            {
                MessageBox.Show("Нет данных для фильтрации.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            var columns = viewModel.GetColumnNames().ToArray();
            string[] operations = { ">", "<", "=", ">=", "<=", "!=" };
            string[] sortMethods = { "По возрастанию", "По убыванию" };

            InitFilterRow(FilteredColumnComboBox, FilterOperationComboBox, columns, operations);
            InitFilterRow(FilteredColumnComboBox2, FilterOperationComboBox2, columns, operations);
            InitFilterRow(FilteredColumnComboBox3, FilterOperationComboBox3, columns, operations);

            SortedColumnComboBox.Items.Clear();
            SortedColumnComboBox.Items.AddRange(columns);

            SortingMethodComboBox.Items.Clear();
            SortingMethodComboBox.Items.AddRange(sortMethods);

            RestoreSavedState();
        }

        private void RestoreSavedState()
        {
            isRestoringState = true;

            var filterRows = new[]
            {
                (FilteredColumnComboBox, FilterOperationComboBox, FilterValueTextBox),
                (FilteredColumnComboBox2, FilterOperationComboBox2, FilterValueTextBox2),
                (FilteredColumnComboBox3, FilterOperationComboBox3, FilterValueTextBox3)
            };

            IReadOnlyList<FilterCondition> savedFilters = viewModel.SavedFormFilters.Count > 0
                ? viewModel.SavedFormFilters
                : viewModel.ActiveFilters;
            SortCondition savedSort = viewModel.SavedFormSort ?? viewModel.ActiveSort;
            for (int i = 0; i < filterRows.Length; i++)
            {
                FilterCondition filter = i < savedFilters.Count ? savedFilters[i] : null;
                if (IsEmptyFilterRow(filter))
                    filter = null;
                RestoreFilterRow(filterRows[i].Item1, filterRows[i].Item2, filterRows[i].Item3, filter);
            }

            if (savedSort != null)
            {
                SortedColumnComboBox.SelectedItem = savedSort.Column;
                SortingMethodComboBox.SelectedItem = savedSort.Ascending ? "По возрастанию" : "По убыванию";
            }
            else
            {
                SortedColumnComboBox.SelectedIndex = -1;
                SortingMethodComboBox.SelectedIndex = -1;
            }

            isRestoringState = false;
        }

        private void SortFilterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (viewModel == null || isRestoringState)
                return;

            viewModel.SaveFormState(CollectFilterRowsDraft(), CollectSortDraft());
        }

        private static void InitFilterRow(ComboBox columnBox, ComboBox operationBox,
            string[] columns, string[] operations)
        {
            columnBox.Items.Clear();
            columnBox.Items.AddRange(columns);
            columnBox.SelectedIndex = -1;

            operationBox.Items.Clear();
            operationBox.Items.AddRange(operations);
            operationBox.SelectedIndex = -1;
        }

        private static void RestoreFilterRow(ComboBox columnBox, ComboBox operationBox,
            TextBox valueBox, FilterCondition filter)
        {
            if (filter == null)
            {
                ClearFilterRow(columnBox, operationBox, valueBox);
                return;
            }

            columnBox.SelectedItem = filter.Column;
            operationBox.SelectedItem = filter.Operation;
            valueBox.Text = filter.Value ?? string.Empty;
        }

        private void ResetFilterButton_Click(object sender, EventArgs e)
        {
            ClearFilterRow(FilteredColumnComboBox, FilterOperationComboBox, FilterValueTextBox);
            ClearFilterRow(FilteredColumnComboBox2, FilterOperationComboBox2, FilterValueTextBox2);
            ClearFilterRow(FilteredColumnComboBox3, FilterOperationComboBox3, FilterValueTextBox3);
        }

        private void ResetSortButton_Click(object sender, EventArgs e)
        {
            SortedColumnComboBox.SelectedIndex = -1;
            SortingMethodComboBox.SelectedIndex = -1;
        }

        private static void ClearFilterRow(ComboBox columnBox, ComboBox operationBox, TextBox valueBox)
        {
            columnBox.SelectedIndex = -1;
            operationBox.SelectedIndex = -1;
            valueBox.Clear();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            var filters = new List<FilterCondition>();

            if (!TryCollectFilterRow(FilteredColumnComboBox, FilterOperationComboBox, FilterValueTextBox, 1, filters))
                return;
            if (!TryCollectFilterRow(FilteredColumnComboBox2, FilterOperationComboBox2, FilterValueTextBox2, 2, filters))
                return;
            if (!TryCollectFilterRow(FilteredColumnComboBox3, FilterOperationComboBox3, FilterValueTextBox3, 3, filters))
                return;

            if (!TryCollectSort(out SortCondition sort))
                return;

            try
            {
                if (viewModel.ApplyFilterAndSort(filters, sort))
                    DialogResult = DialogResult.OK;
                else
                    MessageBox.Show("Не удалось применить настройки.", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат значения для выбранного столбца.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private List<FilterCondition> CollectFilterRowsDraft()
        {
            return new List<FilterCondition>
            {
                CollectFilterRowDraft(FilteredColumnComboBox, FilterOperationComboBox, FilterValueTextBox),
                CollectFilterRowDraft(FilteredColumnComboBox2, FilterOperationComboBox2, FilterValueTextBox2),
                CollectFilterRowDraft(FilteredColumnComboBox3, FilterOperationComboBox3, FilterValueTextBox3)
            };
        }

        private static FilterCondition CollectFilterRowDraft(ComboBox columnBox, ComboBox operationBox, TextBox valueBox)
        {
            string column = columnBox.SelectedIndex >= 0 ? columnBox.SelectedItem?.ToString() : null;
            string operation = operationBox.SelectedIndex >= 0 ? operationBox.SelectedItem?.ToString() : null;
            string value = valueBox.Text;

            if (column == null && operation == null && string.IsNullOrWhiteSpace(value))
                return null;

            return new FilterCondition
            {
                Column = column,
                Operation = operation,
                Value = value
            };
        }

        private static bool IsEmptyFilterRow(FilterCondition filter)
        {
            return filter == null
                || (string.IsNullOrEmpty(filter.Column)
                    && string.IsNullOrEmpty(filter.Operation)
                    && string.IsNullOrWhiteSpace(filter.Value));
        }

        private SortCondition CollectSortDraft()
        {
            if (SortedColumnComboBox.SelectedIndex < 0 && SortingMethodComboBox.SelectedIndex < 0)
                return null;

            string column = SortedColumnComboBox.SelectedIndex >= 0
                ? SortedColumnComboBox.SelectedItem?.ToString()
                : null;
            string method = SortingMethodComboBox.SelectedIndex >= 0
                ? SortingMethodComboBox.SelectedItem?.ToString()
                : null;

            if (column == null && method == null)
                return null;

            return new SortCondition
            {
                Column = column,
                Ascending = method != "По убыванию"
            };
        }

        private static bool TryCollectFilterRow(ComboBox columnBox, ComboBox operationBox, TextBox valueBox,
            int rowNumber, List<FilterCondition> filters)
        {
            bool hasColumn = columnBox.SelectedIndex >= 0;
            bool hasOperation = operationBox.SelectedIndex >= 0;
            string value = valueBox.Text.Trim();
            bool hasValue = !string.IsNullOrEmpty(value);

            if (!hasColumn && !hasOperation && !hasValue)
                return true;

            if (!hasColumn || !hasOperation || !hasValue)
            {
                MessageBox.Show($"Заполните все поля фильтра в строке {rowNumber} или оставьте их пустыми.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            filters.Add(new FilterCondition
            {
                Column = columnBox.SelectedItem.ToString(),
                Operation = operationBox.SelectedItem.ToString(),
                Value = value
            });
            return true;
        }

        private bool TryCollectSort(out SortCondition sort)
        {
            sort = null;
            bool hasColumn = SortedColumnComboBox.SelectedIndex >= 0;
            bool hasMethod = SortingMethodComboBox.SelectedIndex >= 0;

            if (!hasColumn && !hasMethod)
                return true;

            if (!hasColumn || !hasMethod)
            {
                MessageBox.Show("Выберите столбец и способ сортировки или оставьте поля пустыми.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string method = SortingMethodComboBox.SelectedItem.ToString();
            sort = new SortCondition
            {
                Column = SortedColumnComboBox.SelectedItem.ToString(),
                Ascending = method == "По возрастанию"
            };
            return true;
        }
    }
}
