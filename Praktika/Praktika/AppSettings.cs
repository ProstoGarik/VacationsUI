using System.IO;

namespace Praktika
{
    internal static class AppSettings
    {
        public const string DataDbPath = @"C:\Users\ediga\OneDrive\Документы\ОтпускаРабочихТолькоТаблицы.accdb";
        public const string DataDbPassword = "";
        public const string AuthDbFileName = "ОтпускаРабочихПользователи.accdb";
        public const string AuthDbPassword = "";

        public static string AuthDbPath
        {
            get
            {
                string dataDirectory = Path.GetDirectoryName(DataDbPath) ?? AppDomain.CurrentDomain.BaseDirectory;
                return Path.Combine(dataDirectory, AuthDbFileName);
            }
        }
    }
}
