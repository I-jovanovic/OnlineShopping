using System.Text.RegularExpressions;

namespace OnlineShopping.Core.ValueObjects;

/// <summary>
/// Email address value object
/// </summary>
public record EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public EmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email address cannot be null or empty", nameof(email));

        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Invalid email address format", nameof(email));

        Value = email.ToLowerInvariant();
    }

    public static implicit operator string(EmailAddress email) => email.Value;
    public static explicit operator EmailAddress(string email) => new(email);

    public override string ToString() => Value;
}