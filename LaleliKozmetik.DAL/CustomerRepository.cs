using MySql.Data.MySqlClient;

namespace LaleliKozmetik.DAL;

public sealed class CustomerRepository : RepositoryBase
{
    public List<Customer> List()
    {
        var result = new List<Customer>();
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_musteri_listele");
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Customer
            {
                CustomerId = reader.GetInt32("musteri_id"),
                FirstName = reader.GetString("ad"),
                LastName = reader.GetString("soyad"),
                Phone = ReadNullableString(reader, "telefon"),
                Email = ReadNullableString(reader, "eposta"),
                Address = ReadNullableString(reader, "adres")
            });
        }

        return result;
    }

    public void Add(Customer customer)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_musteri_ekle");
        AddCommonParameters(command, customer);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Update(Customer customer)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_musteri_guncelle");
        command.Parameters.AddWithValue("p_musteri_id", customer.CustomerId);
        AddCommonParameters(command, customer);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int customerId)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_musteri_sil");
        command.Parameters.AddWithValue("p_musteri_id", customerId);
        connection.Open();
        command.ExecuteNonQuery();
    }

    private static void AddCommonParameters(MySqlCommand command, Customer customer)
    {
        command.Parameters.AddWithValue("p_ad", customer.FirstName);
        command.Parameters.AddWithValue("p_soyad", customer.LastName);
        command.Parameters.AddWithValue("p_telefon", customer.Phone);
        command.Parameters.AddWithValue("p_eposta", customer.Email);
        command.Parameters.AddWithValue("p_adres", customer.Address);
    }
}
