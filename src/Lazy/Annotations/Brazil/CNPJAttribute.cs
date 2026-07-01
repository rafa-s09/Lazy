namespace Lazy.Annotations.Brazil;

/// <summary>
/// Validates whether a property or field contains a valid Brazilian CNPJ.
/// </summary>
/// <deprecated>
/// Obsolete. Use <see cref="CNPJAlphanumericAttribute"/> instead to support modern alphanumeric formats.
/// </deprecated>
/// <example>
/// <code>
/// public class LegacyCompany
/// {
///     [CNPJ(ErrorMessage = "Invalid CNPJ.")]
///     public string OldTaxId { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false), Obsolete("Use CNPJAlphanumeric instead.")]
public class CNPJAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is string cnpjInput && cnpjInput.IsValidCnpj())
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage ?? "The field is not a valid CNPJ.");
    }
}