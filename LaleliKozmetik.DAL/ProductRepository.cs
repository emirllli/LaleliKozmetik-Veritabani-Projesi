using MySql.Data.MySqlClient;

namespace LaleliKozmetik.DAL;

public sealed class ProductRepository : RepositoryBase
{
    public List<Product> List()
    {
        var result = new List<Product>();
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_urun_listele");
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Product
            {
                ProductId = reader.GetInt32("urun_id"),
                CategoryId = reader.GetInt32("kategori_id"),
                CategoryName = reader.GetString("kategori_adi"),
                ProductName = reader.GetString("urun_adi"),
                Brand = reader.GetString("marka"),
                UnitPrice = reader.GetDecimal("birim_fiyat"),
                VatIncludedPrice = reader.GetDecimal("kdvli_fiyat"),
                StockQuantity = reader.GetInt32("stok_miktari"),
                StockStatus = reader.GetString("stok_durumu"),
                Barcode = ReadNullableString(reader, "barkod")
            });
        }

        return result;
    }

    public Product? GetById(int productId)
    {
        return List().FirstOrDefault(product => product.ProductId == productId);
    }

    public void Add(Product product)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_urun_ekle");
        AddCommonParameters(command, product);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Update(Product product)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_urun_guncelle");
        command.Parameters.AddWithValue("p_urun_id", product.ProductId);
        AddCommonParameters(command, product);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int productId)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_urun_sil");
        command.Parameters.AddWithValue("p_urun_id", productId);
        connection.Open();
        command.ExecuteNonQuery();
    }

    private static void AddCommonParameters(MySqlCommand command, Product product)
    {
        command.Parameters.AddWithValue("p_kategori_id", product.CategoryId);
        command.Parameters.AddWithValue("p_urun_adi", product.ProductName);
        command.Parameters.AddWithValue("p_marka", product.Brand);
        command.Parameters.AddWithValue("p_birim_fiyat", product.UnitPrice);
        command.Parameters.AddWithValue("p_stok_miktari", product.StockQuantity);
        command.Parameters.AddWithValue("p_barkod", product.Barcode);
    }
}
