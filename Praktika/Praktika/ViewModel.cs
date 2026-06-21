using System.Data;
using ClassLibrary;

namespace Praktika
{
    /// <summary>Связь между формами и слоем доступа к данным.</summary>
    public class ViewModel
    {
        private DataManager dataManager;
        private List<FilterCondition> activeFilters = new();
        private SortCondition activeSort;
        private List<FilterCondition> savedFormFilters = new();
        private SortCondition savedFormSort;

        public DataTable VacationsData { get; private set; }

        public string CurrentTable { get; private set; }
        public string CurrentDbPath { get; private set; }
        public string CurrentDbPassword { get; private set; }
        public bool IsFilterActive { get; private set; }
        public bool IsSortActive { get; private set; }
        public IReadOnlyList<FilterCondition> SavedFormFilters => savedFormFilters;
        public SortCondition SavedFormSort => savedFormSort;
        public IReadOnlyList<FilterCondition> ActiveFilters => activeFilters;
        public SortCondition ActiveSort => activeSort;

        public ViewModel()
        {
            dataManager = new DataManager();
        }

        /// <summary>Загружает выбранную таблицу из базы.</summary>
        public void LoadTable(string table, string dbPath, string dbPassword)
        {
            CurrentTable = table;
            CurrentDbPath = dbPath;
            CurrentDbPassword = dbPassword;
            activeFilters.Clear();
            activeSort = null;
            savedFormFilters.Clear();
            savedFormSort = null;
            VacationsData = dataManager.GetTable(table, dbPath, dbPassword);
            IsFilterActive = false;
            IsSortActive = false;
        }

        /// <summary>Применяет фильтр и/или сортировку к текущей таблице.</summary>
        public bool ApplyFilterAndSort(IReadOnlyList<FilterCondition> filters, SortCondition sort)
        {
            activeFilters = filters?.ToList() ?? new List<FilterCondition>();
            activeSort = sort;

            DataTable result = dataManager.GetTableQuery(
                CurrentTable, CurrentDbPath, CurrentDbPassword, activeFilters, activeSort);
            if (result == null)
                return false;

            VacationsData = result;
            IsFilterActive = activeFilters.Count > 0;
            IsSortActive = activeSort != null;
            return true;
        }

        /// <summary>Сохраняет состояние полей формы фильтра/сортировки.</summary>
        public void SaveFormState(IReadOnlyList<FilterCondition> filterRows, SortCondition sort)
        {
            savedFormFilters = filterRows?
                .Select(f => f == null ? null : new FilterCondition
                {
                    Column = f.Column,
                    Operation = f.Operation,
                    Value = f.Value
                })
                .ToList() ?? new List<FilterCondition>();
            savedFormSort = sort == null
                ? null
                : new SortCondition { Column = sort.Column, Ascending = sort.Ascending };
        }

        /// <summary>Сбрасывает фильтр, сохраняя сортировку.</summary>
        public void ClearFilter()
        {
            activeFilters.Clear();
            savedFormFilters.Clear();
            ApplyFilterAndSort(activeFilters, activeSort);
        }

        /// <summary>Сбрасывает сортировку, сохраняя фильтр.</summary>
        public void ClearSort()
        {
            activeSort = null;
            savedFormSort = null;
            ApplyFilterAndSort(activeFilters, activeSort);
        }

        /// <summary>Удаляет строку в БД и в загруженной таблице на форме.</summary>
        public bool DeleteRow(string table, string primaryKeyColumn, object keyValue)
        {
            bool success = dataManager.DeleteRow(table, primaryKeyColumn, keyValue);
            if (success && VacationsData != null)
            {
                string filterExpression = $"[{primaryKeyColumn}] = {keyValue}";
                DataRow[] rows = VacationsData.Select(filterExpression);
                if (rows.Length > 0)
                    rows[0].Delete();
                VacationsData.AcceptChanges();
            }
            return success;
        }

        /// <summary>Добавляет строку и перезагружает текущую таблицу.</summary>
        public bool AddRow(Dictionary<string, object> columnValues)
        {
            bool success = dataManager.InsertRow(CurrentTable, columnValues);
            if (success)
                ReloadCurrentView();
            return success;
        }

        public List<string> GetColumnNames()
        {
            if (VacationsData == null)
                return new List<string>();
            return VacationsData.Columns.Cast<DataColumn>()
                                         .Select(c => c.ColumnName)
                                         .ToList();
        }

        /// <summary>Загружает связанную таблицу (для выпадающих списков).</summary>
        public DataTable GetTableData(string tableName)
        {
            return dataManager.GetTable(tableName, CurrentDbPath, CurrentDbPassword);
        }

        /// <summary>Обновляет строку и перезагружает текущую таблицу.</summary>
        public bool UpdateRow(string table, string primaryKeyColumn, object primaryKeyValue, Dictionary<string, object> columnValues)
        {
            bool success = dataManager.UpdateRow(table, primaryKeyColumn, primaryKeyValue, columnValues);
            if (success)
                ReloadCurrentView();
            return success;
        }

        private void ReloadCurrentView()
        {
            ApplyFilterAndSort(activeFilters, activeSort);
        }

        /// <summary>Выполняет пользовательский SQL-запрос с параметрами.</summary>
        public DataTable ExecuteCustomQuery(string query, Dictionary<string, object> parameters)
        {
            return dataManager.ExecuteQuery(CurrentDbPath, CurrentDbPassword, query, parameters);
        }
    }
}
