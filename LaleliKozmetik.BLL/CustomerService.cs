using LaleliKozmetik.DAL;

namespace LaleliKozmetik.BLL;

public sealed class CustomerService
{
    private readonly CustomerRepository _customers = new();

    public List<Customer> ListCustomers() => _customers.List();

    public void AddCustomer(Customer customer)
    {
        ValidateCustomer(customer);
        _customers.Add(customer);
    }

    public void UpdateCustomer(Customer customer)
    {
        if (customer.CustomerId <= 0) throw new ArgumentException("Guncellenecek musteri secilmelidir.");
        ValidateCustomer(customer);
        _customers.Update(customer);
    }

    public void DeleteCustomer(int customerId)
    {
        if (customerId <= 0) throw new ArgumentException("Silinecek musteri secilmelidir.");
        _customers.Delete(customerId);
    }

    private static void ValidateCustomer(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.FirstName))
        {
            throw new ArgumentException("Musteri adi bos olamaz.");
        }

        if (string.IsNullOrWhiteSpace(customer.LastName))
        {
            throw new ArgumentException("Musteri soyadi bos olamaz.");
        }
    }
}
