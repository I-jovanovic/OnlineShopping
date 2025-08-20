namespace OnlineShopping.Core.Exceptions;

/// <summary>
/// Base domain exception
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key) 
        : base($"{entityName} with key '{key}' was not found.")
    {
    }
}

/// <summary>
/// Exception thrown when business rules are violated
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when insufficient stock is available
/// </summary>
public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int requested, int available)
        : base($"Insufficient stock for '{productName}'. Requested: {requested}, Available: {available}")
    {
    }
}

/// <summary>
/// Exception thrown when trying to create duplicate entities
/// </summary>
public class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string entityName, string field, object value)
        : base($"{entityName} with {field} '{value}' already exists.")
    {
    }
}