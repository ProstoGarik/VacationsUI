using System.Data;
using ClassLibrary;

namespace Praktika
{
    /// <summary>Связь между формами и слоем доступа к данным.</summary>
    public class ViewModel
    {
        private DataManager dataManager;
        public DataTable VacationsData { get; private set; }

        public string CurrentTable { get; private set; }
        public string CurrentDbPath { get; private set; }
        public string CurrentDbPassword { get; private set; }
        public string FilterDescription { get; private set; }

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
            VacationsData = dataManager.GetTable(table, dbPath, dbPassword);
            FilterDescription = null;
        }

        /// <summary>Применяет фильтр к текущей таблице.</summary>
        public bool ApplyFilter(string column, string operation, string value)
        {
            DataTable filtered = dataManager.GetFilteredTable(
                CurrentTable, CurrentDbPath, CurrentDbPassword, column, operation, value);
            if (filtered == null)
                return false;

            VacationsData = filtered;
            FilterDescription = $"{column} {operation} {value}";
            return true;
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
            {
                LoadTable(CurrentTable, CurrentDbPath, CurrentDbPassword);
            }
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
            {
                LoadTable(CurrentTable, CurrentDbPath, CurrentDbPassword);
            }
            return success;
        }
    }
}
