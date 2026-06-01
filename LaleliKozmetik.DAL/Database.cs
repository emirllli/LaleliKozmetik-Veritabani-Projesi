using MySql.Data.MySqlClient;

namespace LaleliKozmetik.DAL;

public static class Database
{
    public static string ConnectionString { get; set; } =
        "Server=localhost;Database=laleli_kozmetik;Uid=root;Pwd=Ketenperep32_;Charset=utf8mb4;";

    public static MySqlConnection CreateConnection()
    {
        return new MySqlConnection(ConnectionString);
    }
}
