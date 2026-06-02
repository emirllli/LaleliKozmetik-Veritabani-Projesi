using System.ComponentModel;

namespace LaleliKozmetik.DAL;

public sealed class Category
{
    [DisplayName("Kategori ID")]
    public int CategoryId { get; set; }

    [DisplayName("Kategori Adi")]
    public string CategoryName { get; set; } = "";

    [DisplayName("Aciklama")]
    public string? Description { get; set; }

    public override string ToString() => CategoryName;
}

public sealed class Product
{
    [DisplayName("Urun ID")]
    public int ProductId { get; set; }

    [DisplayName("Kategori ID")]
    public int CategoryId { get; set; }

    [DisplayName("Kategori")]
    public string CategoryName { get; set; } = "";

    [DisplayName("Urun Adi")]
    public string ProductName { get; set; } = "";

    [DisplayName("Marka")]
    public string Brand { get; set; } = "";

    [DisplayName("Birim Fiyat")]
    public decimal UnitPrice { get; set; }

    [DisplayName("KDV'li Fiyat")]
    public decimal VatIncludedPrice { get; set; }

    [DisplayName("Stok")]
    public int StockQuantity { get; set; }

    [DisplayName("Barkod")]
    public string? Barcode { get; set; }

    public override string ToString() => $"{ProductName} - Stok: {StockQuantity}";
}

public sealed class Customer
{
    [DisplayName("Musteri ID")]
    public int CustomerId { get; set; }

    [DisplayName("Ad")]
    public string FirstName { get; set; } = "";

    [DisplayName("Soyad")]
    public string LastName { get; set; } = "";

    [DisplayName("Telefon")]
    public string? Phone { get; set; }

    [DisplayName("E-posta")]
    public string? Email { get; set; }

    [DisplayName("Adres")]
    public string? Address { get; set; }

    [DisplayName("Ad Soyad")]
    public string FullName => $"{FirstName} {LastName}".Trim();
    public override string ToString() => FullName;
}

public sealed class Sale
{
    [DisplayName("Satis ID")]
    public int SaleId { get; set; }

    [DisplayName("Tarih")]
    public DateTime SaleDate { get; set; }

    [DisplayName("Musteri ID")]
    public int CustomerId { get; set; }

    [DisplayName("Musteri")]
    public string CustomerName { get; set; } = "";

    [DisplayName("Urun ID")]
    public int ProductId { get; set; }

    [DisplayName("Urun")]
    public string ProductName { get; set; } = "";

    [DisplayName("Adet")]
    public int Quantity { get; set; }

    [DisplayName("Birim Fiyat")]
    public decimal UnitPrice { get; set; }

    [DisplayName("Toplam")]
    public decimal TotalAmount { get; set; }
}
