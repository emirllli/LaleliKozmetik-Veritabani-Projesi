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
}
