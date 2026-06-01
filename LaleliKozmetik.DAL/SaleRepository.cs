namespace LaleliKozmetik.DAL;

public sealed class SaleRepository : RepositoryBase
{
    public List<Sale> List()
    {
        var result = new List<Sale>();
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_satis_listele");
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Sale
            {
                SaleId = reader.GetInt32("satis_id"),
                SaleDate = reader.GetDateTime("satis_tarihi"),
                CustomerId = reader.GetInt32("musteri_id"),
                CustomerName = reader.GetString("musteri"),
                ProductId = reader.GetInt32("urun_id"),
                ProductName = reader.GetString("urun_adi"),
                Quantity = reader.GetInt32("adet"),
                UnitPrice = reader.GetDecimal("birim_fiyat"),
                TotalAmount = reader.GetDecimal("toplam_tutar")
            });
        }

        return result;
    }

    public void Add(int productId, int customerId, int quantity)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_satis_ekle");
        command.Parameters.AddWithValue("p_urun_id", productId);
        command.Parameters.AddWithValue("p_musteri_id", customerId);
        command.Parameters.AddWithValue("p_adet", quantity);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Update(int saleId, int productId, int customerId, int quantity)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_satis_guncelle");
        command.Parameters.AddWithValue("p_satis_id", saleId);
        command.Parameters.AddWithValue("p_urun_id", productId);
        command.Parameters.AddWithValue("p_musteri_id", customerId);
        command.Parameters.AddWithValue("p_adet", quantity);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int saleId)
    {
        using var connection = Database.CreateConnection();
        using var command = CreateProcedureCommand(connection, "sp_satis_sil");
        command.Parameters.AddWithValue("p_satis_id", saleId);
        connection.Open();
        command.ExecuteNonQuery();
    }
}
