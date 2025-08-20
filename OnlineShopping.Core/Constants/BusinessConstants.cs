namespace OnlineShopping.Core.Constants;

/// <summary>
/// Business logic constants
/// </summary>
public static class BusinessConstants
{
    public const int MaxCustomerNameLength = 100;
    public const int MaxEmailLength = 256;
    public const int MaxPhoneNumberLength = 20;
    public const int MaxProductNameLength = 200;
    public const int MaxProductDescriptionLength = 2000;
    public const int MaxSkuLength = 50;
    public const int MaxCategoryNameLength = 100;
    public const int MaxCategoryDescriptionLength = 500;
    public const int MaxOrderNotesLength = 1000;
    
    public const decimal MinPrice = 0.01m;
    public const decimal MaxPrice = 999999.99m;
    public const int MinQuantity = 1;
    public const int MaxQuantity = 10000;
    
    public const int CartExpirationDays = 30;
    public const int OrderNumberLength = 12;
    
    public static readonly string[] SupportedPaymentMethods = { "CreditCard", "DebitCard", "PayPal", "BankTransfer" };
}