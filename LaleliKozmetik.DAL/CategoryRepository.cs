using MySql.Data.MySqlClient;

namespace LaleliKozmetik.DAL;

public sealed class CategoryRepository : RepositoryBase
{
    public List<Category> List()
    {
        var result = new List<Category>();
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_kategori_listele");
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Category
            {
                CategoryId = reader.GetInt32("kategori_id"),
                CategoryName = reader.GetString("kategori_adi"),
                Description = ReadNullableString(reader, "aciklama")
            });
        }

        return result;
    }

    public void Add(Category category)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_kategori_ekle");
        command.Parameters.AddWithValue("p_kategori_adi", category.CategoryName);
        command.Parameters.AddWithValue("p_aciklama", category.Description);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Update(Category category)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_kategori_guncelle");
        command.Parameters.AddWithValue("p_kategori_id", category.CategoryId);
        command.Parameters.AddWithValue("p_kategori_adi", category.CategoryName);
        command.Parameters.AddWithValue("p_aciklama", category.Description);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int categoryId)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_kategori_sil");
        command.Parameters.AddWithValue("p_kategori_id", categoryId);
        connection.Open();
        command.ExecuteNonQuery();
    }
}
