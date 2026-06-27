using System.Data;
using System.Data.OleDb;
using System.Security.Cryptography;

namespace ClassLibrary
{
    /// <summary>Данные авторизованного пользователя.</summary>
    public class AuthenticatedUser
    {
        public int ID { get; set; }
        public string Login { get; set; } = string.Empty;
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool CanEditUsers { get; set; }
        public bool CanEditData { get; set; }
    }

    /// <summary>Строка списка пользователей с ролью и правами.</summary>
    public class UserInfo
    {
        public int ID { get; set; }
        public string Login { get; set; } = string.Empty;
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool CanEditUsers { get; set; }
        public bool CanEditData { get; set; }
    }

    /// <summary>Роль пользователя и доступные права.</summary>
    public class RoleInfo
    {
        public int ID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool CanEditUsers { get; set; }
        public bool CanEditData { get; set; }
    }

    /// <summary>Операции авторизации и управления пользователями.</summary>
    public class AuthManager
    {
        private const int Pbkdf2Iterations = 100000;
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const string PasswordHashPrefix = "PBKDF2";

        private readonly string connectionString;

        public AuthManager(string dbPath, string dbPassword = "")
        {
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentException("Путь к базе пользователей не задан.", nameof(dbPath));

            connectionString = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={dbPath};";
            if (!string.IsNullOrEmpty(dbPassword))
                connectionString += $"Jet OLEDB:Database Password={dbPassword};";
        }

        public AuthenticatedUser? Authenticate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
                return null;

            const string query = @"
SELECT [Users].[ID], [Users].[Login], [Users].[PasswordHash], [Users].[RoleID],
       [Roles].[RoleName], [Roles].[CanEditUsers], [Roles].[CanEditData]
FROM [Users]
INNER JOIN [Roles] ON [Users].[RoleID] = [Roles].[ID]
WHERE [Users].[Login] = ?";

            using var conn = new OleDbConnection(connectionString);
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("?", login.Trim());

            conn.Open();
            using OleDbDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            string storedHash = reader["PasswordHash"]?.ToString() ?? string.Empty;
            if (!VerifyPassword(password, storedHash))
                return null;

            return new AuthenticatedUser
            {
                ID = Convert.ToInt32(reader["ID"]),
                Login = reader["Login"].ToString() ?? string.Empty,
                RoleID = Convert.ToInt32(reader["RoleID"]),
                RoleName = reader["RoleName"].ToString() ?? string.Empty,
                CanEditUsers = Convert.ToBoolean(reader["CanEditUsers"]),
                CanEditData = Convert.ToBoolean(reader["CanEditData"])
            };
        }

        public List<UserInfo> GetUsers()
        {
            const string query = @"
SELECT [Users].[ID], [Users].[Login], [Users].[RoleID],
       [Roles].[RoleName], [Roles].[CanEditUsers], [Roles].[CanEditData]
FROM [Users]
INNER JOIN [Roles] ON [Users].[RoleID] = [Roles].[ID]
ORDER BY [Users].[Login]";

            var users = new List<UserInfo>();
            using var conn = new OleDbConnection(connectionString);
            using var cmd = new OleDbCommand(query, conn);

            conn.Open();
            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new UserInfo
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    Login = reader["Login"].ToString() ?? string.Empty,
                    RoleID = Convert.ToInt32(reader["RoleID"]),
                    RoleName = reader["RoleName"].ToString() ?? string.Empty,
                    CanEditUsers = Convert.ToBoolean(reader["CanEditUsers"]),
                    CanEditData = Convert.ToBoolean(reader["CanEditData"])
                });
            }

            return users;
        }

        public List<RoleInfo> GetRoles()
        {
            const string query = @"
SELECT [ID], [RoleName], [CanEditUsers], [CanEditData]
FROM [Roles]
ORDER BY [RoleName]";

            var roles = new List<RoleInfo>();
            using var conn = new OleDbConnection(connectionString);
            using var cmd = new OleDbCommand(query, conn);

            conn.Open();
            using OleDbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add(new RoleInfo
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    RoleName = reader["RoleName"].ToString() ?? string.Empty,
                    CanEditUsers = Convert.ToBoolean(reader["CanEditUsers"]),
                    CanEditData = Convert.ToBoolean(reader["CanEditData"])
                });
            }

            return roles;
        }

        public bool UserExists(string login)
        {
            const string query = "SELECT COUNT(*) FROM [Users] WHERE [Login] = ?";
            using var conn = new OleDbConnection(connectionString);
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("?", login.Trim());

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public int CreateUser(string login, string password, int roleId)
        {
            string passwordHash = HashPassword(password);
            return CreateUserWithPasswordHash(login, passwordHash, roleId);
        }

        public int CreateUserWithPasswordHash(string login, string passwordHash, int roleId)
        {
            const string query = @"
INSERT INTO [Users] ([Login], [PasswordHash], [RoleID])
VALUES (?, ?, ?)";

            using var conn = new OleDbConnection(connectionString);
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("?", login.Trim());
            cmd.Parameters.AddWithValue("?", passwordHash);
            cmd.Parameters.AddWithValue("?", roleId);

            conn.Open();
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT @@IDENTITY";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdateUserLogin(int userId, string login)
        {
            const string query = "UPDATE [Users] SET [Login] = ? WHERE [ID] = ?";
            return ExecuteNonQuery(query, login.Trim(), userId) > 0;
        }

        public bool UpdateUserPassword(int userId, string password)
        {
            const string query = "UPDATE [Users] SET [PasswordHash] = ? WHERE [ID] = ?";
            return ExecuteNonQuery(query, HashPassword(password), userId) > 0;
        }

        public bool UpdateUserRole(int userId, int roleId)
        {
            const string query = "UPDATE [Users] SET [RoleID] = ? WHERE [ID] = ?";
            return ExecuteNonQuery(query, roleId, userId) > 0;
        }

        public bool DeleteUser(int userId)
        {
            const string query = "DELETE FROM [Users] WHERE [ID] = ?";
            return ExecuteNonQuery(query, userId) > 0;
        }

        public int CreateRole(string roleName, bool canEditUsers, bool canEditData)
        {
            const string query = @"
INSERT INTO [Roles] ([RoleName], [CanEditUsers], [CanEditData])
VALUES (?, ?, ?)";

            using var conn = new OleDbConnection(connectionString);
            using var cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("?", roleName.Trim());
            cmd.Parameters.Add("?", OleDbType.Boolean).Value = canEditUsers;
            cmd.Parameters.Add("?", OleDbType.Boolean).Value = canEditData;

            conn.Open();
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT @@IDENTITY";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdateRole(int roleId, string roleName, bool canEditUsers, bool canEditData)
        {
            const string query = @"
UPDATE [Roles]
SET [RoleName] = ?, [CanEditUsers] = ?, [CanEditData] = ?
WHERE [ID] = ?";

            return ExecuteNonQuery(query, roleName.Trim(), canEditUsers, canEditData, roleId) > 0;
        }

        public bool DeleteRole(int roleId)
        {
            const string query = "DELETE FROM [Roles] WHERE [ID] = ?";
            return ExecuteNonQuery(query, roleId) > 0;
        }

        public bool HasUsers()
        {
            const string query = "SELECT COUNT(*) FROM [Users]";
            using var conn = new OleDbConnection(connectionString);
            using var cmd = new OleDbCommand(query, conn);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static string HashPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Pbkdf2Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return $"{PasswordHashPrefix}${Pbkdf2Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            string[] parts = storedHash.Split('$');
            if (parts.Length != 4 || parts[0] != PasswordHashPrefix)
                return false;

            try
            {
                int iterations = int.Parse(parts[1]);
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expectedHash = Convert.FromBase64String(parts[3]);
                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private int ExecuteNonQuery(string query, params object[] parameters)
        {
            using var conn = new OleDbConnection(connectionString);
            using var cmd = new OleDbCommand(query, conn);

            foreach (object parameter in parameters)
            {
                cmd.Parameters.AddWithValue("?", parameter ?? DBNull.Value);
            }

            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
