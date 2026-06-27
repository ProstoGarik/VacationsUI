using System.Data;
using System.Data.OleDb;
using System.Diagnostics;

namespace ClassLibrary
{
    /// <summary>Операции чтения и изменения данных в Access.</summary>
    public class DataManager
    {
        private readonly ConnectionManager connectionManager;

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

        /// <summary>Загружает таблицу с фильтрацией и/или сортировкой. Возвращает null при ошибке.</summary>
        public DataTable GetTableQuery(string table, string dbPath, string dbPassword,
            IReadOnlyList<FilterCondition> filters, SortCondition sort)
        {
            bool connected = connectionManager.ConnectToDatabase(dbPath, dbPassword);
            if (!connected)
                return null;

            var parameters = new List<OleDbParameter>();
            string query = $"SELECT * FROM [{table}]";

            if (filters != null && filters.Count > 0)
            {
                var clauses = new List<string>();
                foreach (var filter in filters)
                {
                    if (!AllowedFilterOperations.Contains(filter.Operation))
                        return null;

                    OleDbType? columnType = GetColumnOleDbType(table, filter.Column);
                    if (columnType == null)
                        return null;

                    string sqlOperation = filter.Operation == "!=" ? "<>" : filter.Operation;
                    clauses.Add($"[{filter.Column}] {sqlOperation} ?");

                    var param = new OleDbParameter("?", ConvertFilterValue(filter.Value, columnType.Value) ?? DBNull.Value);
                    param.OleDbType = columnType.Value;
                    parameters.Add(param);
                }
                query += " WHERE " + string.Join(" AND ", clauses);
            }

            if (sort != null)
            {
                OleDbType? sortColumnType = GetColumnOleDbType(table, sort.Column);
                if (sortColumnType == null)
                    return null;

                query += $" ORDER BY [{sort.Column}] {(sort.Ascending ? "ASC" : "DESC")}";
            }

            DataTable dataTable = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connectionManager.ConnectionString))
            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                if (parameters.Count > 0)
                    cmd.Parameters.AddRange(parameters.ToArray());

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
            DataTable schema = GetTableSchema(table);
            DebugWriteSchema(table, schema);

            var columns = new List<string>();
            var parameters = new List<OleDbParameter>();

            foreach (var kvp in columnValues)
            {
                if (kvp.Key == "ID")
                    continue;

                DataRow[] rows = schema.Select($"COLUMN_NAME = '{kvp.Key}'");
                if (rows.Length == 0)
                {
                    Debug.WriteLine($"[DataManager] InsertRow skipped unknown column '{kvp.Key}' for table '{table}'.");
                    continue;
                }

                OleDbType oleDbType = MapDataType(rows[0]["DATA_TYPE"]);

                columns.Add($"[{kvp.Key}]");
                var param = new OleDbParameter("?", ConvertParameterValue(kvp.Value, oleDbType))
                {
                    OleDbType = oleDbType,
                    SourceColumn = kvp.Key
                };
                parameters.Add(param);
            }

            string columnsPart = string.Join(", ", columns);
            string valuesPart = string.Join(", ", parameters.Select(p => "?"));
            string query = $"INSERT INTO [{table}] ({columnsPart}) VALUES ({valuesPart})";

            using (var conn = new OleDbConnection(connectionManager.ConnectionString))
            using (var cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters.ToArray());
                try
                {
                    DebugWriteCommand("InsertRow", table, query, cmd.Parameters);
                    conn.Open();
                    int affected = cmd.ExecuteNonQuery();
                    Debug.WriteLine($"[DataManager] InsertRow affected rows: {affected}");
                    return affected > 0;
                }
                catch (OleDbException ex)
                {
                    DebugWriteOleDbException("InsertRow", table, query, cmd.Parameters, ex);
                    throw;
                }
            }
        }

        /// <summary>Обновляет строку по первичному ключу.</summary>
        public bool UpdateRow(string table, string primaryKeyColumn, object primaryKeyValue, Dictionary<string, object> columnValues)
        {
            DataTable schema = GetTableSchema(table);

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
            DataRow[] rows = GetTableSchema(table).Select($"COLUMN_NAME = '{column}'");
            return rows.Length == 0 ? null : MapDataType(rows[0]["DATA_TYPE"]);
        }

        private DataTable GetTableSchema(string table)
        {
            using var conn = new OleDbConnection(connectionManager.ConnectionString);
            conn.Open();
            return conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, table, null })
                ?? new DataTable();
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
        private OleDbType MapDataType(object dataType)
        {
            string dataTypeText = dataType?.ToString() ?? string.Empty;
            if (int.TryParse(dataTypeText, out int oleDbTypeCode))
                return NormalizeOleDbType((OleDbType)oleDbTypeCode);

            switch (dataTypeText)
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

        private static OleDbType NormalizeOleDbType(OleDbType oleDbType)
        {
            switch (oleDbType)
            {
                case OleDbType.WChar:
                    return OleDbType.VarWChar;
                case OleDbType.Char:
                    return OleDbType.VarChar;
                default:
                    return oleDbType;
            }
        }

        private static object ConvertParameterValue(object value, OleDbType oleDbType)
        {
            if (value == null || value == DBNull.Value)
                return DBNull.Value;

            switch (oleDbType)
            {
                case OleDbType.Boolean:
                    return Convert.ToBoolean(value);
                case OleDbType.Integer:
                    return Convert.ToInt32(value);
                case OleDbType.BigInt:
                    return Convert.ToInt64(value);
                case OleDbType.Double:
                case OleDbType.Currency:
                    return Convert.ToDouble(value);
                case OleDbType.Date:
                case OleDbType.DBDate:
                case OleDbType.DBTimeStamp:
                    return Convert.ToDateTime(value);
                default:
                    return value;
            }
        }

        /// <summary>Выполняет произвольный SQL-запрос с параметрами.</summary>
        public DataTable ExecuteQuery(string dbPath, string dbPassword, string query, Dictionary<string, object> parameters = null)
        {
            bool connected = connectionManager.ConnectToDatabase(dbPath, dbPassword);
            if (!connected)
                return null;

            DataTable dataTable = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connectionManager.ConnectionString))
            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                if (parameters != null)
                {
                    foreach (var kvp in parameters)
                    {
                        cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                    }
                }
                conn.Open();
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private static void DebugWriteSchema(string table, DataTable schema)
        {
            Debug.WriteLine($"[DataManager] Schema for table '{table}':");
            if (schema == null)
            {
                Debug.WriteLine("[DataManager]   Schema is null.");
                return;
            }

            foreach (DataRow row in schema.Rows)
            {
                Debug.WriteLine(
                    "[DataManager]   " +
                    $"Column='{row["COLUMN_NAME"]}', " +
                    $"DATA_TYPE='{row["DATA_TYPE"]}', " +
                    $"TYPE_NAME='{GetSchemaValue(row, "TYPE_NAME")}', " +
                    $"IS_NULLABLE='{GetSchemaValue(row, "IS_NULLABLE")}', " +
                    $"ORDINAL_POSITION='{GetSchemaValue(row, "ORDINAL_POSITION")}'");
            }
        }

        private static object GetSchemaValue(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) ? row[column] : string.Empty;
        }

        private static void DebugWriteCommand(string operation, string table, string query, OleDbParameterCollection parameters)
        {
            Debug.WriteLine($"[DataManager] {operation}");
            Debug.WriteLine($"[DataManager]   Table: {table}");
            Debug.WriteLine($"[DataManager]   Query: {query}");
            Debug.WriteLine($"[DataManager]   Parameters count: {parameters.Count}");

            for (int i = 0; i < parameters.Count; i++)
            {
                OleDbParameter parameter = parameters[i];
                object value = parameter.Value;
                string clrType = value == null || value == DBNull.Value ? "null" : value.GetType().FullName;

                Debug.WriteLine(
                    "[DataManager]   " +
                    $"Param[{i}], " +
                    $"Column='{parameter.SourceColumn}', " +
                    $"OleDbType='{parameter.OleDbType}', " +
                    $"DbType='{parameter.DbType}', " +
                    $"ClrType='{clrType}', " +
                    $"Value='{FormatParameterValue(parameter)}'");
            }
        }

        private static string FormatParameterValue(OleDbParameter parameter)
        {
            if (!string.IsNullOrEmpty(parameter.SourceColumn)
                && parameter.SourceColumn.Contains("Password", StringComparison.OrdinalIgnoreCase))
                return "<redacted>";

            object value = parameter.Value;
            if (value == null || value == DBNull.Value)
                return "DBNull";

            return value.ToString();
        }

        private static void DebugWriteOleDbException(string operation, string table, string query,
            OleDbParameterCollection parameters, OleDbException ex)
        {
            Debug.WriteLine($"[DataManager] {operation} failed for table '{table}'.");
            DebugWriteCommand(operation + " failed command", table, query, parameters);
            Debug.WriteLine($"[DataManager]   Exception: {ex}");

            for (int i = 0; i < ex.Errors.Count; i++)
            {
                OleDbError error = ex.Errors[i];
                Debug.WriteLine(
                    "[DataManager]   " +
                    $"OleDbError[{i}], " +
                    $"NativeError='{error.NativeError}', " +
                    $"SQLState='{error.SQLState}', " +
                    $"Source='{error.Source}', " +
                    $"Message='{error.Message}'");
            }
        }
    }
}
