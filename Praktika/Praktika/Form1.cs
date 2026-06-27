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
        private const string UsersTableName = "Users";
        private const string RolesTableName = "Roles";
        private const string DeleteButtonColumnName = "DeleteButton";
        private const string EditButtonColumnName = "EditButton";
        private const string VacationBySpecialtyQueryName = "Отпуска по специальности";
        private const string NoFilterStatus = "Фильтр: Отсутствует";
        private const string NoSortStatus = "Сортировка: Отсутствует";
        private const int WdAlignParagraphCenter = 1;
        private const int WdAutoFitContent = 1;
        private const int WdDoNotSaveChanges = 0;

        private string currentDataDbPath;
        private string currentDataDbPassword;
        private bool isDataDbConnected;

        private ViewModel viewModel;
        private AuthenticatedUser? currentUser;

        public MainForm()
        {
            InitializeComponent();
            viewModel = new ViewModel();
            ConfigureDataGridViews();
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            GoToControlTabButton.Click += GoToControlTabButton_Click;
        }

        private void ConfigureDataGridViews()
        {
            ConfigureReadOnlyDataGridView(tableDataGridView);
            ConfigureReadOnlyDataGridView(quieryTableDataGridView);
        }

        private static void ConfigureReadOnlyDataGridView(DataGridView dataGridView)
        {
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeTableSelector();
            InitializeQuerySelector();

            currentDataDbPath = null;
            currentDataDbPassword = string.Empty;
            isDataDbConnected = false;

            ClearDataUi();
            tabControl1.SelectedTab = ControlTabPage;
            UpdateAccountDataLabel();
            UpdateConnectionUI();
            UpdateDataTabsAvailabilityNotice();
            ApplyUserPermissions();
        }

        private void InitializeTableSelector()
        {
            TableSelectComboBox.Items.Clear();
            TableSelectComboBox.Items.AddRange(new object[] { "Отпуска", "Рабочие", "Подразделения" });

            if (TableSelectComboBox.Items.Count > 0)
                TableSelectComboBox.SelectedIndex = 0;
        }

        private void AddAuthTablesToSelector()
        {
            if (!CanEditUsers())
                return;

            if (!TableSelectComboBox.Items.Contains(UsersTableName))
                TableSelectComboBox.Items.Add(UsersTableName);

            if (!TableSelectComboBox.Items.Contains(RolesTableName))
                TableSelectComboBox.Items.Add(RolesTableName);
        }

        private void InitializeQuerySelector()
        {
            quierySelectComboBox.Items.Clear();
            quierySelectComboBox.Items.Add(VacationBySpecialtyQueryName);
            quierySelectComboBox.SelectedIndex = 0;
        }

        private void TableSelectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void RefreshTable()
        {
            string selectedTable = TableSelectComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTable))
                return;

            LoadTable(selectedTable);
        }

        private void LoadTable(string table)
        {
            if (string.IsNullOrEmpty(currentDataDbPath))
            {
                ClearDataUi();
                return;
            }

            viewModel.LoadTable(table, currentDataDbPath, currentDataDbPassword);
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
            RemoveActionButtons();

            if (!CanEditCurrentTable())
                return;

            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn
            {
                Name = DeleteButtonColumnName,
                HeaderText = "",
                Text = "Удалить",
                UseColumnTextForButtonValue = true,
                Width = 85,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            tableDataGridView.Columns.Add(deleteColumn);

            if (IsCurrentUsersTable())
                return;

            DataGridViewButtonColumn editColumn = new DataGridViewButtonColumn
            {
                Name = EditButtonColumnName,
                HeaderText = "",
                Text = "Изменить",
                UseColumnTextForButtonValue = true,
                Width = 85,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            tableDataGridView.Columns.Add(editColumn);
        }

        private void AutoResizeColumns()
        {
            foreach (DataGridViewColumn col in tableDataGridView.Columns)
            {
                if (col.Name != DeleteButtonColumnName && col.Name != EditButtonColumnName)
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void TableDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender != tableDataGridView || !CanEditCurrentTable())
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewColumn clickedColumn = tableDataGridView.Columns[e.ColumnIndex];
            DataGridViewRow row = tableDataGridView.Rows[e.RowIndex];
            if (row.DataBoundItem is not DataRowView dataRow)
                return;

            if (clickedColumn.Name == DeleteButtonColumnName)
            {
                DeleteSelectedRow(dataRow);
            }
            else if (clickedColumn.Name == EditButtonColumnName)
            {
                EditSelectedRow(dataRow);
            }
        }

        private void DeleteSelectedRow(DataRowView dataRow)
        {
            DialogResult result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;

            if (DeleteRow(dataRow))
                MessageBox.Show("Запись удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Ошибка при удалении.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void EditSelectedRow(DataRowView dataRow)
        {
            DataTable table = dataRow.DataView.Table;
            string primaryKeyColumn = GetPrimaryKeyColumn(table);

            using (AddEditForm editForm = new AddEditForm())
            {
                editForm.SetViewModel(viewModel);
                editForm.IsEditMode = true;
                editForm.PrimaryKeyValue = dataRow[primaryKeyColumn];
                editForm.EditValues = GetEditValues(dataRow, primaryKeyColumn);

                if (editForm.ShowDialog(this) == DialogResult.OK)
                    RefreshTable();
            }
        }

        private static string GetPrimaryKeyColumn(DataTable table)
        {
            return table.PrimaryKey.Length > 0
                ? table.PrimaryKey[0].ColumnName
                : table.Columns[0].ColumnName;
        }

        private static Dictionary<string, object> GetEditValues(DataRowView dataRow, string primaryKeyColumn)
        {
            var values = new Dictionary<string, object>();
            foreach (DataColumn column in dataRow.DataView.Table.Columns)
            {
                if (column.ColumnName != primaryKeyColumn)
                    values[column.ColumnName] = dataRow[column.ColumnName];
            }

            return values;
        }

        private bool DeleteRow(DataRowView dataRow)
        {
            string currentTable = TableSelectComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(currentTable))
                return false;

            DataTable dt = dataRow.DataView.Table;
            string pkColumn = GetPrimaryKeyColumn(dt);
            object pkValue = dataRow[pkColumn];

            return viewModel.DeleteRow(currentTable, pkColumn, pkValue);
        }

        private void AddRowButton_Click(object sender, EventArgs e)
        {
            if (!CanEditCurrentTable())
            {
                MessageBox.Show("У текущего пользователя нет прав на изменение выбранной таблицы.", "Доступ запрещен",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
            ApplyUserPermissions();
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
            RefreshSelectedQuery();
        }

        private void RefreshSelectedQuery()
        {
            string selected = quierySelectComboBox.SelectedItem?.ToString();
            if (selected == VacationBySpecialtyQueryName)
            {
                LoadSpecialties();
            }
            else
            {
                quieryParamComboBox.DataSource = null;
                quieryTableDataGridView.DataSource = null;
            }
        }

        private void LoadSpecialties()
        {
            if (!isDataDbConnected)
            {
                quieryParamComboBox.DataSource = null;
                quieryTableDataGridView.DataSource = null;
                return;
            }

            DataTable workers = viewModel.GetTableData("Рабочие");
            if (workers != null && workers.Rows.Count > 0)
            {
                var specialties = workers.AsEnumerable()
                                         .Select(row => row.Field<string>("Специальность"))
                                         .Where(s => !string.IsNullOrEmpty(s))
                                         .Distinct()
                                         .ToList();
                quieryParamComboBox.DataSource = specialties;
                quieryParamComboBox.DisplayMember = "ToString";
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

        private void CreateReportButton_Click(object sender, EventArgs e)
        {
            if (quieryTableDataGridView.DataSource is not DataTable reportData || reportData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для формирования отчёта.", "Отчёт",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string queryName = quierySelectComboBox.SelectedItem?.ToString() ?? "Запрос";
            string parameter = quieryParamComboBox.SelectedItem?.ToString() ?? string.Empty;
            string title = string.IsNullOrWhiteSpace(parameter)
                ? queryName
                : $"{queryName} - {parameter}";

            try
            {
                CreateWordReport(title, reportData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сформировать отчёт Word: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CreateWordReport(string title, DataTable reportData)
        {
            Type? wordType = Type.GetTypeFromProgID("Word.Application");
            if (wordType == null)
                throw new InvalidOperationException("Microsoft Word не установлен или недоступен через COM.");

            dynamic wordApp = Activator.CreateInstance(wordType)
                ?? throw new InvalidOperationException("Не удалось запустить Microsoft Word.");
            dynamic document = wordApp.Documents.Add();

            try
            {
                dynamic titleParagraph = document.Paragraphs.Add();
                titleParagraph.Range.Text = title;
                titleParagraph.Range.Font.Bold = 1;
                titleParagraph.Range.Font.Size = 12;
                titleParagraph.Alignment = WdAlignParagraphCenter;
                titleParagraph.Range.InsertParagraphAfter();

                dynamic tableRange = document.Paragraphs.Add().Range;
                dynamic table = document.Tables.Add(
                    tableRange,
                    reportData.Rows.Count + 1,
                    reportData.Columns.Count);

                table.Borders.Enable = 1;

                for (int columnIndex = 0; columnIndex < reportData.Columns.Count; columnIndex++)
                {
                    dynamic cell = table.Cell(1, columnIndex + 1);
                    cell.Range.Text = reportData.Columns[columnIndex].ColumnName;
                    cell.Range.Bold = 0;
                }

                for (int rowIndex = 0; rowIndex < reportData.Rows.Count; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < reportData.Columns.Count; columnIndex++)
                    {
                        object value = reportData.Rows[rowIndex][columnIndex];
                        table.Cell(rowIndex + 2, columnIndex + 1).Range.Text = FormatReportValue(value);
                    }
                }

                table.Range.Font.Bold = 0;
                table.Range.Font.Size = 12;

                table.AutoFitBehavior(WdAutoFitContent);
                wordApp.Visible = true;
            }
            catch
            {
                document.Close(WdDoNotSaveChanges);
                wordApp.Quit();
                throw;
            }
        }

        private static string FormatReportValue(object value)
        {
            if (value == DBNull.Value || value == null)
                return string.Empty;

            return value is DateTime dateTime
                ? dateTime.ToString("dd.MM.yyyy")
                : value.ToString() ?? string.Empty;
        }

        private bool CanEditData()
        {
            return currentUser?.CanEditData == true;
        }

        private bool CanEditUsers()
        {
            return currentUser?.CanEditUsers == true;
        }

        private bool IsAuthTable(string table)
        {
            return table == UsersTableName || table == RolesTableName;
        }

        private bool IsCurrentAuthTable()
        {
            string selectedTable = TableSelectComboBox.SelectedItem?.ToString();
            return !string.IsNullOrEmpty(selectedTable) && IsAuthTable(selectedTable);
        }

        private bool IsCurrentUsersTable()
        {
            return TableSelectComboBox.SelectedItem?.ToString() == UsersTableName;
        }

        private bool CanEditCurrentTable()
        {
            if (!isDataDbConnected) return false;
            if (currentUser == null) return false;
            return IsCurrentAuthTable() ? CanEditUsers() : CanEditData();
        }

        private void ApplyUserPermissions()
        {
            bool canEditCurrentTable = CanEditCurrentTable();
            AddRowButton.Enabled = canEditCurrentTable;

            if (!canEditCurrentTable)
            {
                RemoveActionButtons();
            }
        }

        private void UpdateConnectionUI()
        {
            if (isDataDbConnected)
            {
                ConnectionDynamicButton.Text = "Закрыть подключение";
                ConnectionStatusLabel.Text = "Подключение к БД: Выполнено";
            }
            else
            {
                ConnectionDynamicButton.Text = "Выбрать файл";
                ConnectionStatusLabel.Text = "Подключение к БД: Не выполнено";
            }

            LogoutButton.Enabled = currentUser != null;
        }

        private void UpdateAccountDataLabel()
        {
            string login = string.IsNullOrWhiteSpace(currentUser?.Login)
                ? "неизвестно"
                : currentUser.Login;

            AccountDataLabel.Text = $"Выполнен вход как: {login}";
        }

        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateDataTabsAvailabilityNotice();
        }

        private void GoToControlTabButton_Click(object? sender, EventArgs e)
        {
            tabControl1.SelectedTab = ControlTabPage;
        }

        private void UpdateDataTabsAvailabilityNotice()
        {
            UpdateDataTabsContentVisibility();

            bool shouldShowNotice = !isDataDbConnected && IsDataTabSelected();
            if (shouldShowNotice)
            {
                MoveDataTabAvailabilityNoticeToSelectedTab();
                TabNotAvailableLabel.BringToFront();
                GoToControlTabButton.BringToFront();
            }

            TabNotAvailableLabel.Visible = shouldShowNotice;
            GoToControlTabButton.Visible = shouldShowNotice;
        }

        private void UpdateDataTabsContentVisibility()
        {
            SetTabContentVisibility(tablesTabPage, isDataDbConnected);
            SetTabContentVisibility(quieryTabPage, isDataDbConnected);
        }

        private void SetTabContentVisibility(TabPage tabPage, bool isVisible)
        {
            foreach (Control control in tabPage.Controls)
            {
                if (control == TabNotAvailableLabel || control == GoToControlTabButton)
                    continue;

                control.Visible = isVisible;
            }
        }

        private bool IsDataTabSelected()
        {
            return tabControl1.SelectedTab == tablesTabPage || tabControl1.SelectedTab == quieryTabPage;
        }

        private void MoveDataTabAvailabilityNoticeToSelectedTab()
        {
            TabPage targetTab = tabControl1.SelectedTab == quieryTabPage
                ? quieryTabPage
                : tablesTabPage;

            if (TabNotAvailableLabel.Parent != targetTab)
                targetTab.Controls.Add(TabNotAvailableLabel);

            if (GoToControlTabButton.Parent != targetTab)
                targetTab.Controls.Add(GoToControlTabButton);
        }

        private void RemoveActionButtons()
        {
            RemoveColumnIfExists(DeleteButtonColumnName);
            RemoveColumnIfExists(EditButtonColumnName);
        }

        private void RemoveColumnIfExists(string columnName)
        {
            if (tableDataGridView.Columns[columnName] != null)
                tableDataGridView.Columns.Remove(columnName);
        }

        private void ClearDataUi()
        {
            tableDataGridView.DataSource = null;
            quieryTableDataGridView.DataSource = null;
            quieryParamComboBox.DataSource = null;
            RemoveActionButtons();
            AddRowButton.Enabled = false;
            FilterStatusLabel.Text = NoFilterStatus;
            SortStatusLabel.Text = NoSortStatus;
            quieryFilterStatusLabel.Text = NoFilterStatus;
            quierySortStatusLabel.Text = NoSortStatus;
        }

        private void DisconnectData()
        {
            viewModel.DisconnectData();
            currentDataDbPath = null;
            currentDataDbPassword = string.Empty;
            isDataDbConnected = false;
            currentUser = null;
            ClearDataUi();
            InitializeTableSelector();
            tabControl1.SelectedTab = ControlTabPage;
            UpdateAccountDataLabel();
            UpdateConnectionUI();
            UpdateDataTabsAvailabilityNotice();
            ApplyUserPermissions();
        }

        private void ConnectionDynamicButton_Click(object sender, EventArgs e)
        {
            if (isDataDbConnected)
            {
                DisconnectData();
                return;
            }

            ConnectDataFromFile();
        }

        private void ConnectDataFromFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файлы Access (*.accdb)|*.accdb|Все файлы (*.*)|*.*";
                openFileDialog.Title = "Выберите базу данных";
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                currentDataDbPath = openFileDialog.FileName;
                currentDataDbPassword = string.Empty;
                viewModel.ConfigureAuthDatabase(currentDataDbPath, currentDataDbPassword);

                using (LoginForm loginForm = new LoginForm(currentDataDbPath, currentDataDbPassword))
                {
                    if (loginForm.ShowDialog(this) != DialogResult.OK || loginForm.CurrentUser == null)
                    {
                        DisconnectData();
                        return;
                    }

                    currentUser = loginForm.CurrentUser;
                }

                AddAuthTablesToSelector();
                isDataDbConnected = true;
                RefreshTable();

                if (viewModel.VacationsData != null)
                {
                    UpdateAccountDataLabel();
                    UpdateConnectionUI();
                    UpdateDataTabsAvailabilityNotice();
                    UpdateStatusLabels();
                    RefreshSelectedQuery();
                    ApplyUserPermissions();
                    tabControl1.SelectedTab = tablesTabPage;
                    return;
                }

                MessageBox.Show("Не удалось подключиться к выбранной базе данных или загрузить таблицу.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisconnectData();
            }
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            DisconnectData();
        }
    }
}
