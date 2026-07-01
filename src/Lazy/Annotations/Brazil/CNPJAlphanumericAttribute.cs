namespace Lazy.Annotations.Brazil;

/// <summary>
/// Validates whether a property or field contains a valid Brazilian CNPJ (Cadastro Nacional da Pessoa Jurídica), 
/// supporting the modern alphanumeric format standard.
/// </summary>
/// <remarks>
/// This attribute allows null values. Combine it with <see cref="RequiredAttribute"/> if the field is mandatory.
/// </remarks>
/// <example>
/// <code>
/// public class Company
/// {
///     [CNPJAlphanumeric(ErrorMessage = "The company registry is invalid.")]
///     public string TaxId { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class CNPJAlphanumericAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is string cnpjInput && cnpjInput.IsValidCnpjAlphanumeric())
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage ?? "The field is not a valid CNPJ.");
    }
}