namespace OnlineShopping.Core.Interfaces;

/// <summary>
/// Validation service interface
/// </summary>
public interface IValidationService : IService
{
    bool IsValidEmail(string email);
    bool IsValidPhoneNumber(string? phoneNumber);
    bool IsValidPrice(decimal price);
    bool IsValidQuantity(int quantity);
    bool IsValidSku(string sku);
    void ValidateCustomerData(string email, string firstName, string lastName);
    void ValidateProductData(string name, decimal price, string sku, int stockQuantity);
    void ValidateAddressData(string street, string city, string state, string country, string postalCode);
}