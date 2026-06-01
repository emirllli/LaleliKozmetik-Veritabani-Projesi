using LaleliKozmetik.DAL;

namespace LaleliKozmetik.BLL;

public sealed class SaleService
{
    private readonly ProductRepository _products = new();
    private readonly CustomerRepository _customers = new();
    private readonly SaleRepository _sales = new();

    public List<Product> ListProducts() => _products.List();

    public List<Customer> ListCustomers() => _customers.List();

    public List<Sale> ListSales() => _sales.List();

    public void SellProduct(int productId, int customerId, int quantity)
    {
        if (productId <= 0)
        {
            throw new ArgumentException("Urun secilmelidir.");
        }

        if (customerId <= 0)
        {
            throw new ArgumentException("Musteri secilmelidir.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Satis adedi 0'dan buyuk olmalidir.");
        }

        var product = _products.GetById(productId);
        if (product is null)
        {
            throw new ArgumentException("Secilen urun bulunamadi.");
        }

        if (product.StockQuantity <= 0)
        {
            throw new InvalidOperationException("Stokta olmayan urun satilamaz.");
        }

        if (product.StockQuantity < quantity)
        {
            throw new InvalidOperationException("Satis adedi mevcut stoktan fazla olamaz.");
        }

        _sales.Add(productId, customerId, quantity);
    }

    public void UpdateSale(int saleId, int productId, int customerId, int quantity)
    {
        if (saleId <= 0) throw new ArgumentException("Guncellenecek satis secilmelidir.");
        if (productId <= 0) throw new ArgumentException("Urun secilmelidir.");
        if (customerId <= 0) throw new ArgumentException("Musteri secilmelidir.");
        if (quantity <= 0) throw new ArgumentException("Satis adedi 0'dan buyuk olmalidir.");

        _sales.Update(saleId, productId, customerId, quantity);
    }

    public void DeleteSale(int saleId)
    {
        if (saleId <= 0) throw new ArgumentException("Silinecek satis secilmelidir.");
        _sales.Delete(saleId);
    }
}
