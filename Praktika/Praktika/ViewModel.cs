using System.Data;
using ClassLibrary;

namespace Praktika
{
    public class ViewModel
    {
        private DataManager dataManager;

        public ViewModel()
        {
            dataManager = new DataManager();
        }
        public DataTable VacationsData { get; private set; }

        /// <summary>
        /// Загружает данные. Требуется вызвать перед связыванием
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="dbPassword"></param>
        public void LoadTable(string table, string dbPath, string dbPassword)
        {
            VacationsData = dataManager.GetTable(table, dbPath, dbPassword);
        }

        public bool DeleteRow(string table, string primaryKeyColumn, object keyValue)
        {
            bool success = dataManager.DeleteRow(table, primaryKeyColumn, keyValue);
            if (success && VacationsData != null)
            {
                // Удаление записи в интерфейсе пользователя
                string filterExpression = $"[{primaryKeyColumn}] = {keyValue}";
                DataRow[] rows = VacationsData.Select(filterExpression);
                if (rows.Length > 0)
                    rows[0].Delete();
                VacationsData.AcceptChanges();
            }
            return success;
        }
    }
}