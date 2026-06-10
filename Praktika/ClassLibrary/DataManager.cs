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

        /// <summary>
        /// Удаляет запись из указанной таблицы по значению первичного ключа
        /// </summary>
        /// <param name="table">Имя таблицы</param>
        /// <param name="primaryKeyColumn">Имя столбца первичного ключа (например, "ID")</param>
        /// <param name="keyValue">Значение первичного ключа удаляемой строки</param>
        /// <returns>True если удаление успешно, иначе False</returns>
        public bool DeleteRow(string table, string primaryKeyColumn, object keyValue)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionManager.ConnectionString))
            {
                string query = $"DELETE FROM [{table}] WHERE [{primaryKeyColumn}] = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", keyValue);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}