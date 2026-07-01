namespace Lazy.Annotations.Brazil;

/// <summary>
/// Validates whether a property or field contains a valid Brazilian CPF (Cadastro de Pessoas Físicas) number.
/// </summary>
/// <remarks>
/// This attribute allows null values. Combine it with <see cref="RequiredAttribute"/> if the field is mandatory.
/// </remarks>
/// <example>
/// <code>
/// public class Customer
/// {
///     [Required]
///     [CPFAttribute(ErrorMessage = "Please provide a valid CPF.")]
///     public string NationalId { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class CPFAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) 
            return ValidationResult.Success; 

        if (value is string cpfInput && cpfInput.IsValidCpf())                
            return ValidationResult.Success;        

        return new ValidationResult(ErrorMessage ?? "The field is not a valid CPF.");
    }
}