using System.Data;
using MySql.Data.MySqlClient;

namespace LaleliKozmetik.DAL;

public abstract class RepositoryBase
{
    protected static MySqlCommand CreateProcedureCommand(MySqlConnection connection, string procedureName)
    {
        return new MySqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
    }

    protected static string? ReadNullableString(MySqlDataReader reader, string column)
    {
        int index = reader.GetOrdinal(column);
        return reader.IsDBNull(index) ? null : reader.GetString(index);
    }

    protected static string ReadColumnOrDefault(MySqlDataReader reader, string column, string defaultValue = "")
    {
        try
        {
            int index = reader.GetOrdinal(column);
            return reader.IsDBNull(index) ? defaultValue : reader.GetString(index);
        }
        catch (IndexOutOfRangeException)
        {
            return defaultValue;
        }
    }
}
