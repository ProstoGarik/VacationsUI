using System.Data;
using System.Data.OleDb;

namespace ClassLibrary
{
    /// <summary>Операции чтения и изменения данных в Access.</summary>
    public class DataManager
    {
        private ConnectionManager connectionManager;

        public DataManager()
        {
            connectionManager = new ConnectionManager();
        }

        private static readonly HashSet<string> AllowedFilterOperations = new()
        {
            ">", "<", "=", ">=", "<=", "!="
        };

        /// <summary>Загружает таблицу. Возвращает null при ошибке подключения.</summary>
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

        /// <summary>Загружает таблицу с фильтром по столбцу. Возвращает null при ошибке.</summary>
        public DataTable GetFilteredTable(string table, string dbPath, string dbPassword,
            string column, string operation, string value)
        {
            if (!AllowedFilterOperations.Contains(operation))
                return null;

            bool connected = connectionManager.ConnectToDatabase(dbPath, dbPassword);
            if (!connected)
                return null;

            OleDbType? columnType = GetColumnOleDbType(table, column);
            if (columnType == null)
                return null;

            object paramValue = ConvertFilterValue(value, columnType.Value);
            string sqlOperation = operation == "!=" ? "<>" : operation;
            string query = $"SELECT * FROM [{table}] WHERE [{column}] {sqlOperation} ?";

            DataTable dataTable = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connectionManager.ConnectionString))
            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                var param = new OleDbParameter("?", paramValue ?? DBNull.Value);
                param.OleDbType = columnType.Value;
                cmd.Parameters.Add(param);
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        /// <summary>Удаляет строку по значению первичного ключа.</summary>
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

        /// <summary>Добавляет строку; типы параметров определяются по схеме таблицы.</summary>
        public bool InsertRow(string table, Dictionary<string, object> columnValues)
        {
            DataTable schema = null;
            using (var conn = new OleDbConnection(connectionManager.ConnectionString))
            {
                conn.Open();
                schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, table, null });
                conn.Close();
            }

            var columns = new List<string>();
            var parameters = new List<OleDbParameter>();

            foreach (var kvp in columnValues)
            {
                // ID в Access обычно автоинкрементный
                if (kvp.Key == "ID")
                    continue;

                DataRow[] rows = schema.Select($"COLUMN_NAME = '{kvp.Key}'");
                if (rows.Length == 0)
                    continue;

                string dataType = rows[0]["DATA_TYPE"].ToString();
                OleDbType oleDbType = MapDataType(dataType);

                columns.Add($"[{kvp.Key}]");
                var param = new OleDbParameter("?", kvp.Value ?? DBNull.Value);
                param.OleDbType = oleDbType;
                parameters.Add(param);
            }

            string columnsPart = string.Join(", ", columns);
            string valuesPart = string.Join(", ", parameters.Select(p => "?"));
            string query = $"INSERT INTO [{table}] ({columnsPart}) VALUES ({valuesPart})";

            using (var conn = new OleDbConnection(connectionManager.ConnectionString))
            using (var cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                conn.Open();
                int affected = cmd.ExecuteNonQuery();
                return affected > 0;
            }
        }

        /// <summary>Обновляет строку по первичному ключу.</summary>
        public bool UpdateRow(string table, string primaryKeyColumn, object primaryKeyValue, Dictionary<string, object> columnValues)
        {
            DataTable schema;
            using (var conn = new OleDbConnection(connectionManager.ConnectionString))
            {
                conn.Open();
                schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, table, null });
                conn.Close();
            }

            var setClauses = new List<string>();
            var parameters = new List<OleDbParameter>();

            foreach (var kvp in columnValues)
            {
                if (kvp.Key == primaryKeyColumn) continue;

                DataRow[] rows = schema.Select($"COLUMN_NAME = '{kvp.Key}'");
                if (rows.Length == 0) continue;

                string dataType = rows[0]["DATA_TYPE"].ToString();
                OleDbType oleDbType = MapDataType(dataType);

                setClauses.Add($"[{kvp.Key}] = ?");
                var param = new OleDbParameter("?", kvp.Value ?? DBNull.Value);
                param.OleDbType = oleDbType;
                parameters.Add(param);
            }

            DataRow[] pkRows = schema.Select($"COLUMN_NAME = '{primaryKeyColumn}'");
            string pkDataType = pkRows.Length > 0 ? pkRows[0]["DATA_TYPE"].ToString() : "DBTYPE_I4";
            OleDbType pkOleDbType = MapDataType(pkDataType);
            var pkParam = new OleDbParameter("?", primaryKeyValue ?? DBNull.Value);
            pkParam.OleDbType = pkOleDbType;
            parameters.Add(pkParam);

            string setPart = string.Join(", ", setClauses);
            string query = $"UPDATE [{table}] SET {setPart} WHERE [{primaryKeyColumn}] = ?";

            using (var conn = new OleDbConnection(connectionManager.ConnectionString))
            using (var cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                conn.Open();
                int affected = cmd.ExecuteNonQuery();
                return affected > 0;
            }
        }

        private OleDbType? GetColumnOleDbType(string table, string column)
        {
            using (var conn = new OleDbConnection(connectionManager.ConnectionString))
            {
                conn.Open();
                var schema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, table, null });
                DataRow[] rows = schema.Select($"COLUMN_NAME = '{column}'");
                if (rows.Length == 0)
                    return null;
                return MapDataType(rows[0]["DATA_TYPE"].ToString());
            }
        }

        private static object ConvertFilterValue(string value, OleDbType oleDbType)
        {
            switch (oleDbType)
            {
                case OleDbType.Integer:
                    return int.Parse(value);
                case OleDbType.BigInt:
                    return long.Parse(value);
                case OleDbType.Double:
                case OleDbType.Currency:
                    return double.Parse(value);
                case OleDbType.Date:
                    return DateTime.Parse(value);
                case OleDbType.Boolean:
                    return bool.Parse(value);
                default:
                    return value;
            }
        }

        /// <summary>Соответствие типов столбцов Access и OleDb.</summary>
        private OleDbType MapDataType(string dataType)
        {
            switch (dataType)
            {
                case "DBTYPE_I4": return OleDbType.Integer;
                case "DBTYPE_I8": return OleDbType.BigInt;
                case "DBTYPE_R8": return OleDbType.Double;
                case "DBTYPE_CY": return OleDbType.Currency;
                case "DBTYPE_DATE": return OleDbType.Date;
                case "DBTYPE_BSTR": return OleDbType.VarWChar;
                case "DBTYPE_BOOL": return OleDbType.Boolean;
                default: return OleDbType.Variant;
            }
        }
    }
}
