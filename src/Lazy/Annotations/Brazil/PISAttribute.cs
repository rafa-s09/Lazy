namespace Lazy.Annotations.Brazil;


/// <summary>
/// Validates whether a property or field contains a valid Brazilian PIS (Programa de Integração Social) number.
/// </summary>
/// <remarks>
/// This attribute allows null values. Combine it with <see cref="RequiredAttribute"/> if the field is mandatory.
/// </remarks>
/// <example>
/// <code>
/// public class Employee
/// {
///     [PIS(ErrorMessage = "Invalid PIS number.")]
///     public string PisNumber { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class PISAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if (value is string pisInput && pisInput.IsValidPis())
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage ?? "The field is not a valid PIS.");
    }
}