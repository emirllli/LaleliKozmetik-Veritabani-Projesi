using LaleliKozmetik.DAL;

namespace LaleliKozmetik.BLL;

public sealed class ProductService
{
    private readonly ProductRepository _products = new();
    private readonly CategoryRepository _categories = new();

    public List<Product> ListProducts() => _products.List();

    public List<Category> ListCategories() => _categories.List();

    public void AddCategory(string name, string? description)
    {
        ValidateCategory(name);
        _categories.Add(new Category { CategoryName = name.Trim(), Description = description });
    }

    public void UpdateCategory(int id, string name, string? description)
    {
        if (id <= 0) throw new ArgumentException("Guncellenecek kategori secilmelidir.");
        ValidateCategory(name);
        _categories.Update(new Category { CategoryId = id, CategoryName = name.Trim(), Description = description });
    }

    public void DeleteCategory(int id)
    {
        if (id <= 0) throw new ArgumentException("Silinecek kategori secilmelidir.");
        _categories.Delete(id);
    }

    private static void ValidateCategory(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Kategori adi bos olamaz.");
        }
    }

    public void SaveProduct(Product product)
    {
        ValidateProduct(product);
        if (product.ProductId == 0)
        {
            _products.Add(product);
        }
        else
        {
            _products.Update(product);
        }
    }

    public void DeleteProduct(int productId)
    {
        if (productId <= 0)
        {
            throw new ArgumentException("Silinecek urun secilmelidir.");
        }

        _products.Delete(productId);
    }

    private static void ValidateProduct(Product product)
    {
        if (product.CategoryId <= 0)
        {
            throw new ArgumentException("Kategori secilmelidir.");
        }

        if (string.IsNullOrWhiteSpace(product.ProductName))
        {
            throw new ArgumentException("Urun adi bos olamaz.");
        }

        if (string.IsNullOrWhiteSpace(product.Brand))
        {
            throw new ArgumentException("Marka bos olamaz.");
        }

        if (product.UnitPrice < 0)
        {
            throw new ArgumentException("Birim fiyat negatif olamaz.");
        }

        if (product.StockQuantity < 0)
        {
            throw new ArgumentException("Stok miktari negatif olamaz.");
        }
    }
}
