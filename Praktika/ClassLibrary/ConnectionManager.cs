using System.Data.OleDb;
using System.Diagnostics;

namespace ClassLibrary
{
    /// <summary>Управление подключением к базе Access.</summary>
    public class ConnectionManager
    {
        private string connectionString;
        private string currentDbPath;
        private string currentDbPassword;
        private bool isDbConnected;

        public string ConnectionString => connectionString;
        public bool IsDbConnected => isDbConnected;
        public string CurrentDbPath => currentDbPath;
        public string CurrentDbPassword => currentDbPassword;

        /// <summary>Подключается к базе по пути и паролю.</summary>
        public bool ConnectToDatabase(string dbPath, string dbPassword)
        {
            string ConnectionString = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={dbPath};";

            if (!string.IsNullOrEmpty(dbPassword))
                ConnectionString += $"Jet OLEDB:Database Password={dbPassword};";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(ConnectionString))
                {
                    conn.Open();
                    currentDbPath = dbPath;
                    currentDbPassword = dbPassword;
                    connectionString = ConnectionString;
                    isDbConnected = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка подключения к БД: {ex.Message}");
                isDbConnected = false;
                return false;
            }
        }
    }
}
