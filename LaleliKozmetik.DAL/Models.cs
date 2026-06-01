namespace LaleliKozmetik.DAL;

public sealed class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public string? Description { get; set; }

    public override string ToString() => CategoryName;
}

public sealed class Product
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string Brand { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public decimal VatIncludedPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? Barcode { get; set; }

    public override string ToString() => $"{ProductName} - Stok: {StockQuantity}";
}

public sealed class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
    public override string ToString() => FullName;
}

public sealed class Sale
{
    public int SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = "";
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
}
