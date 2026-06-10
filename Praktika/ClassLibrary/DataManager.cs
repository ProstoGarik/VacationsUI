using System.Data;
using System.Data.OleDb;

namespace ClassLibrary
{
    public class DataManager
    {
        private ConnectionManager connectionManager;

        public DataManager()
        {
            connectionManager = new ConnectionManager();
        }

        /// <summary>
        /// Возвращает объект DataTable с данными из таблицы
        /// </summary>
        /// <param name="dbPath"></param>
        /// <param name="dbPassword"></param>
        /// <returns></returns>
        public DataTable GetTable(string table, string dbPath, string dbPassword)
        {
            bool connected = connectionManager.ConnectToDatabase(dbPath, dbPassword);
            if (!connected)
                return null;

            string connString = connectionManager.ConnectionString;
            DataTable dataTable = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                string query = $"SELECT * FROM [{table}]";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn))
                {
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
    }
}