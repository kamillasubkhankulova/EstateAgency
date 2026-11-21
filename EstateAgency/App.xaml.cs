using System.Windows;

namespace EstateAgency
{
    public partial class App : Application
    {
        /// <summary>Строка подключения к локальной БД.</summary>
        public static string ConnectionString { get; } =
            "Server=(localdb)\\MSSQLLocalDB;Database=EstateAgencyDB;Trusted_Connection=True;";
    }
}