using LaleliKozmetik.BLL;
using LaleliKozmetik.DAL;

namespace LaleliKozmetik.UI;

public partial class Form1 : Form
{
    private readonly ProductService _productService = new();
    private readonly CustomerService _customerService = new();
    private readonly SaleService _saleService = new();

    private readonly DataGridView _productGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private readonly DataGridView _customerGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private readonly DataGridView _saleGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

    private readonly ComboBox _categoryCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _productNameText = new();
    private readonly TextBox _brandText = new();
    private readonly NumericUpDown _priceInput = new() { DecimalPlaces = 2, Maximum = 100000, Width = 120 };
    private readonly NumericUpDown _stockInput = new() { Maximum = 100000, Width = 120 };
    private readonly TextBox _barcodeText = new();

    private readonly TextBox _customerFirstNameText = new();
    private readonly TextBox _customerLastNameText = new();
    private readonly TextBox _phoneText = new();
    private readonly TextBox _emailText = new();
    private readonly TextBox _addressText = new();

    private readonly ComboBox _saleProductCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _saleCustomerCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _saleQuantityInput = new() { Minimum = 1, Maximum = 10000, Width = 120 };

    private readonly DataGridView _categoryGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private readonly TextBox _categoryNameText = new();
    private readonly TextBox _categoryDescText = new();

    private static readonly Color PrimaryColor = Color.FromArgb(74, 20, 140);   // Deep Purple
    private static readonly Color SecondaryColor = Color.FromArgb(173, 20, 87); // Pink/Rose
    private static readonly Color BgColor = Color.FromArgb(250, 248, 255);      // Lilac White

    public Form1()
    {
        InitializeComponent();
        this.Font = new Font("Segoe UI", 10);
        this.BackColor = BgColor;
        this.Text = "Laleli Kozmetik - Stok ve Satış Otomasyonu";
        
        ApplyGridStyle(_productGrid);
        ApplyGridStyle(_customerGrid);
        ApplyGridStyle(_saleGrid);
        ApplyGridStyle(_categoryGrid);

        BuildInterface();
        LoadAllData();
    }

    private void ApplyGridStyle(DataGridView grid)
    {
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10);
        grid.ColumnHeadersHeight = 35;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 242, 255);
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    private void ApplyButtonStyle(Button btn, Color backColor)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.BackColor = backColor;
        btn.ForeColor = Color.White;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = new Font("Segoe UI Semibold", 9);
        btn.Cursor = Cursors.Hand;
        btn.Height = 35;
    }

    private void BuildInterface()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(15, 8) };
        tabs.TabPages.Add(BuildProductTab());
        tabs.TabPages.Add(BuildCustomerTab());
        tabs.TabPages.Add(BuildSaleTab());
        tabs.TabPages.Add(BuildCategoryTab());
        Controls.Add(tabs);
    }

    private TabPage BuildCategoryTab()
    {
        var page = new TabPage("🌸 Kategoriler");
        var panel = new TableLayoutPanel { Dock = DockStyle.Top, Height = 130, ColumnCount = 4, RowCount = 2, Padding = new Padding(15) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        AddField(panel, "Kategori Adi:", _categoryNameText, 0, 0);
        AddField(panel, "Aciklama:", _categoryDescText, 2, 0);

        var addButton = new Button { Text = "✚ Ekle", Width = 100 };
        var updateButton = new Button { Text = "📝 Guncelle", Width = 100 };
        var deleteButton = new Button { Text = "🗑 Sil", Width = 100 };
        var refreshButton = new Button { Text = "🔄 Yenile", Width = 100 };

        ApplyButtonStyle(addButton, Color.FromArgb(46, 125, 50)); // Green
        ApplyButtonStyle(updateButton, Color.FromArgb(21, 101, 192)); // Blue
        ApplyButtonStyle(deleteButton, Color.FromArgb(198, 40, 40)); // Red
        ApplyButtonStyle(refreshButton, Color.FromArgb(84, 110, 122)); // Gray

        addButton.Click += (_, _) => SaveCategory(0);
        updateButton.Click += (_, _) => SaveSelectedCategory();
        deleteButton.Click += (_, _) => DeleteSelectedCategory();
        refreshButton.Click += (_, _) => LoadAllData();

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 0) };
        buttons.Controls.AddRange(new Control[] { addButton, updateButton, deleteButton, refreshButton });
        panel.Controls.Add(buttons, 1, 1);
        panel.SetColumnSpan(buttons, 3);

        _categoryGrid.SelectionChanged += (_, _) => FillSelectedCategory();
        page.Controls.Add(_categoryGrid);
        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildProductTab()
    {
        var page = new TabPage("💄 Urun Yonetimi");
        var panel = new TableLayoutPanel { Dock = DockStyle.Top, Height = 180, ColumnCount = 6, RowCount = 3, Padding = new Padding(15) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));

        AddField(panel, "Kategori:", _categoryCombo, 0, 0);
        AddField(panel, "Urun Adi:", _productNameText, 2, 0);
        AddField(panel, "Marka:", _brandText, 4, 0);
        AddField(panel, "Fiyat:", _priceInput, 0, 1);
        AddField(panel, "Stok:", _stockInput, 2, 1);
        AddField(panel, "Barkod:", _barcodeText, 4, 1);

        var addButton = new Button { Text = "✚ Urun Ekle", Width = 110 };
        var updateButton = new Button { Text = "📝 Guncelle", Width = 110 };
        var deleteButton = new Button { Text = "🗑 Sil", Width = 110 };
        var refreshButton = new Button { Text = "🔄 Listele", Width = 110 };

        ApplyButtonStyle(addButton, Color.FromArgb(46, 125, 50));
        ApplyButtonStyle(updateButton, Color.FromArgb(21, 101, 192));
        ApplyButtonStyle(deleteButton, Color.FromArgb(198, 40, 40));
        ApplyButtonStyle(refreshButton, Color.FromArgb(84, 110, 122));

        addButton.Click += (_, _) => SaveProduct(0);
        updateButton.Click += (_, _) => SaveSelectedProduct();
        deleteButton.Click += (_, _) => DeleteSelectedProduct();
        refreshButton.Click += (_, _) => LoadAllData();

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 0) };
        buttons.Controls.AddRange(new Control[] { addButton, updateButton, deleteButton, refreshButton });
        panel.Controls.Add(buttons, 1, 2);
        panel.SetColumnSpan(buttons, 5);

        _productGrid.SelectionChanged += (_, _) => FillSelectedProduct();
        page.Controls.Add(_productGrid);
        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildCustomerTab()
    {
        var page = new TabPage("👥 Musteri Kaydi");
        var panel = new TableLayoutPanel { Dock = DockStyle.Top, Height = 160, ColumnCount = 6, RowCount = 3, Padding = new Padding(15) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));

        AddField(panel, "Ad:", _customerFirstNameText, 0, 0);
        AddField(panel, "Soyad:", _customerLastNameText, 2, 0);
        AddField(panel, "Telefon:", _phoneText, 4, 0);
        AddField(panel, "E-posta:", _emailText, 0, 1);
        AddField(panel, "Adres:", _addressText, 2, 1);

        var addButton = new Button { Text = "✚ Musteri Kaydet", Width = 140 };
        var updateButton = new Button { Text = "📝 Guncelle", Width = 100 };
        var deleteButton = new Button { Text = "🗑 Sil", Width = 100 };
        var refreshButton = new Button { Text = "🔄 Listele", Width = 100 };

        ApplyButtonStyle(addButton, Color.FromArgb(46, 125, 50));
        ApplyButtonStyle(updateButton, Color.FromArgb(21, 101, 192));
        ApplyButtonStyle(deleteButton, Color.FromArgb(198, 40, 40));
        ApplyButtonStyle(refreshButton, Color.FromArgb(84, 110, 122));

        addButton.Click += (_, _) => SaveCustomer(0);
        updateButton.Click += (_, _) => SaveSelectedCustomer();
        deleteButton.Click += (_, _) => DeleteSelectedCustomer();
        refreshButton.Click += (_, _) => LoadAllData();

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 0) };
        buttons.Controls.AddRange(new Control[] { addButton, updateButton, deleteButton, refreshButton });
        panel.Controls.Add(buttons, 1, 2);
        panel.SetColumnSpan(buttons, 5);

        _customerGrid.SelectionChanged += (_, _) => FillSelectedCustomer();
        page.Controls.Add(_customerGrid);
        page.Controls.Add(panel);
        return page;
    }

    private TabPage BuildSaleTab()
    {
        var page = new TabPage("💰 Satis Ekrani");
        var panel = new TableLayoutPanel { Dock = DockStyle.Top, Height = 130, ColumnCount = 6, RowCount = 2, Padding = new Padding(15) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

        AddField(panel, "Urun Sec:", _saleProductCombo, 0, 0);
        AddField(panel, "Musteri:", _saleCustomerCombo, 2, 0);
        AddField(panel, "Adet:", _saleQuantityInput, 4, 0);

        var sellButton = new Button { Text = "🛒 Satis Yap", Width = 120 };
        var updateButton = new Button { Text = "📝 Duzenle", Width = 110 };
        var deleteButton = new Button { Text = "🗑 İptal Et", Width = 110 };
        var refreshButton = new Button { Text = "🔄 Yenile", Width = 100 };

        ApplyButtonStyle(sellButton, SecondaryColor);
        ApplyButtonStyle(updateButton, Color.FromArgb(21, 101, 192));
        ApplyButtonStyle(deleteButton, Color.FromArgb(198, 40, 40));
        ApplyButtonStyle(refreshButton, Color.FromArgb(84, 110, 122));

        sellButton.Click += (_, _) => SellProduct();
        updateButton.Click += (_, _) => UpdateSelectedSale();
        deleteButton.Click += (_, _) => DeleteSelectedSale();
        refreshButton.Click += (_, _) => LoadAllData();

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 0) };
        buttons.Controls.AddRange(new Control[] { sellButton, updateButton, deleteButton, refreshButton });
        panel.Controls.Add(buttons, 1, 1);
        panel.SetColumnSpan(buttons, 5);

        _saleGrid.SelectionChanged += (_, _) => FillSelectedSale();
        page.Controls.Add(_saleGrid);
        page.Controls.Add(panel);
        return page;
    }

    private static void AddField(TableLayoutPanel panel, string labelText, Control input, int column, int row)
    {
        panel.Controls.Add(new Label { Text = labelText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI Semibold", 9) }, column, row);
        input.Dock = DockStyle.Fill;
        panel.Controls.Add(input, column + 1, row);
    }

    private void LoadAllData()
    {
        RunSafely(() =>
        {
            var categories = _productService.ListCategories();
            _categoryCombo.DataSource = categories;
            _categoryCombo.DisplayMember = nameof(Category.CategoryName);
            _categoryCombo.ValueMember = nameof(Category.CategoryId);
            _categoryGrid.DataSource = categories.ToList();

            var products = _productService.ListProducts();
            _productGrid.DataSource = products;
            _saleProductCombo.DataSource = products.ToList();
            _saleProductCombo.DisplayMember = nameof(Product.ProductName);
            _saleProductCombo.ValueMember = nameof(Product.ProductId);

            var customers = _customerService.ListCustomers();
            _customerGrid.DataSource = customers;
            _saleCustomerCombo.DataSource = customers.ToList();
            _saleCustomerCombo.DisplayMember = nameof(Customer.FullName);
            _saleCustomerCombo.ValueMember = nameof(Customer.CustomerId);

            _saleGrid.DataSource = _saleService.ListSales();
        });
    }

    private void SaveCategory(int id)
    {
        RunSafely(() =>
        {
            if (id == 0) _productService.AddCategory(_categoryNameText.Text, _categoryDescText.Text);
            else _productService.UpdateCategory(id, _categoryNameText.Text, _categoryDescText.Text);
            _categoryNameText.Clear();
            _categoryDescText.Clear();
            LoadAllData();
        });
    }

    private void SaveSelectedCategory()
    {
        if (_categoryGrid.CurrentRow?.DataBoundItem is Category cat)
        {
            SaveCategory(cat.CategoryId);
        }
    }

    private void DeleteSelectedCategory()
    {
        if (_categoryGrid.CurrentRow?.DataBoundItem is Category cat)
        {
            RunSafely(() =>
            {
                _productService.DeleteCategory(cat.CategoryId);
                LoadAllData();
            });
        }
    }

    private void FillSelectedCategory()
    {
        if (_categoryGrid.CurrentRow?.DataBoundItem is Category cat)
        {
            _categoryNameText.Text = cat.CategoryName;
            _categoryDescText.Text = cat.Description;
        }
    }

    private void SaveProduct(int productId)
    {
        RunSafely(() =>
        {
            _productService.SaveProduct(new Product
            {
                ProductId = productId,
                CategoryId = Convert.ToInt32(_categoryCombo.SelectedValue),
                ProductName = _productNameText.Text.Trim(),
                Brand = _brandText.Text.Trim(),
                UnitPrice = _priceInput.Value,
                StockQuantity = Convert.ToInt32(_stockInput.Value),
                Barcode = string.IsNullOrWhiteSpace(_barcodeText.Text) ? null : _barcodeText.Text.Trim()
            });
            ClearProductInputs();
            LoadAllData();
        });
    }

    private void SaveSelectedProduct()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is Product product)
        {
            SaveProduct(product.ProductId);
        }
    }

    private void DeleteSelectedProduct()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is not Product product)
        {
            return;
        }

        RunSafely(() =>
        {
            _productService.DeleteProduct(product.ProductId);
            ClearProductInputs();
            LoadAllData();
        });
    }

    private void FillSelectedProduct()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is not Product product || _categoryCombo.DataSource is null)
        {
            return;
        }

        _categoryCombo.SelectedValue = product.CategoryId;
        _productNameText.Text = product.ProductName;
        _brandText.Text = product.Brand;
        _priceInput.Value = product.UnitPrice;
        _stockInput.Value = product.StockQuantity;
        _barcodeText.Text = product.Barcode ?? "";
    }

    private void SaveCustomer(int customerId)
    {
        RunSafely(() =>
        {
            var customer = new Customer
            {
                CustomerId = customerId,
                FirstName = _customerFirstNameText.Text.Trim(),
                LastName = _customerLastNameText.Text.Trim(),
                Phone = _phoneText.Text.Trim(),
                Email = _emailText.Text.Trim(),
                Address = _addressText.Text.Trim()
            };

            if (customerId == 0) _customerService.AddCustomer(customer);
            else _customerService.UpdateCustomer(customer);

            ClearCustomerInputs();
            LoadAllData();
        });
    }

    private void SaveSelectedCustomer()
    {
        if (_customerGrid.CurrentRow?.DataBoundItem is Customer customer)
        {
            SaveCustomer(customer.CustomerId);
        }
    }

    private void DeleteSelectedCustomer()
    {
        if (_customerGrid.CurrentRow?.DataBoundItem is Customer customer)
        {
            RunSafely(() =>
            {
                _customerService.DeleteCustomer(customer.CustomerId);
                ClearCustomerInputs();
                LoadAllData();
            });
        }
    }

    private void FillSelectedCustomer()
    {
        if (_customerGrid.CurrentRow?.DataBoundItem is Customer customer)
        {
            _customerFirstNameText.Text = customer.FirstName;
            _customerLastNameText.Text = customer.LastName;
            _phoneText.Text = customer.Phone;
            _emailText.Text = customer.Email;
            _addressText.Text = customer.Address;
        }
    }

    private void SellProduct()
    {
        RunSafely(() =>
        {
            _saleService.SellProduct(
                Convert.ToInt32(_saleProductCombo.SelectedValue),
                Convert.ToInt32(_saleCustomerCombo.SelectedValue),
                Convert.ToInt32(_saleQuantityInput.Value));
            LoadAllData();
        });
    }

    private void UpdateSelectedSale()
    {
        if (_saleGrid.CurrentRow?.DataBoundItem is Sale sale)
        {
            RunSafely(() =>
            {
                _saleService.UpdateSale(
                    sale.SaleId,
                    Convert.ToInt32(_saleProductCombo.SelectedValue),
                    Convert.ToInt32(_saleCustomerCombo.SelectedValue),
                    Convert.ToInt32(_saleQuantityInput.Value));
                LoadAllData();
            });
        }
    }

    private void DeleteSelectedSale()
    {
        if (_saleGrid.CurrentRow?.DataBoundItem is Sale sale)
        {
            RunSafely(() =>
            {
                _saleService.DeleteSale(sale.SaleId);
                LoadAllData();
            });
        }
    }

    private void FillSelectedSale()
    {
        if (_saleGrid.CurrentRow?.DataBoundItem is Sale sale)
        {
            _saleProductCombo.SelectedValue = sale.ProductId;
            _saleCustomerCombo.SelectedValue = sale.CustomerId;
            _saleQuantityInput.Value = sale.Quantity;
        }
    }

    private void ClearProductInputs()
    {
        _productNameText.Clear();
        _brandText.Clear();
        _priceInput.Value = 0;
        _stockInput.Value = 0;
        _barcodeText.Clear();
    }

    private void ClearCustomerInputs()
    {
        _customerFirstNameText.Clear();
        _customerLastNameText.Clear();
        _phoneText.Clear();
        _emailText.Clear();
        _addressText.Clear();
    }

    private static void RunSafely(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Islem Uyarisi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
