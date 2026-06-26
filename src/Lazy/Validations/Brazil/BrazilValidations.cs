namespace Lazy.Validations.Brazil;

/// <summary>
/// Provides high-performance validation methods for Brazilian government documents including CPF, CNPJ, and PIS.
/// This class leverages source-generated regular expressions and memory-efficient ReadOnlySpan operations to eliminate heap allocations.
/// </summary>
public static partial class Validations
{
    #region Regex

    /// <summary>
    /// Regular expression used to validate that a CPF contains exactly 11 digits after symbol stripping.
    /// </summary>
    [GeneratedRegex(@"^\d{11}$")]
    private static partial Regex CPFRegex();

    /// <summary>
    /// Regular expression used to validate that a CNPJ contains exactly 14 digits after symbol stripping.
    /// </summary>
    [GeneratedRegex(@"^\d{14}$")]
    private static partial Regex CNPJRegex();

    /// <summary>
    /// Regular expression used to validate that a PIS contains exactly 11 digits after symbol stripping.
    /// </summary>
    [GeneratedRegex(@"^\d{11}$")]
    private static partial Regex PISRegex();

    #endregion Regex

    #region Validates

    /// <summary>
    /// Validates a CPF (Brazilian individual taxpayer registry identification number) using a high-performance checksum algorithm.
    /// </summary>
    /// <remarks>
    /// The input string is first sanitized using ClearSymbols to strip dots, dashes, and other formatting characters.
    /// If the clean text length is not exactly eleven characters or contains non-numeric symbols, validation fails immediately.
    /// To block false positives from common test data, uniform digit sequences such as all zeros or all ones are explicitly rejected.
    /// The algorithm uses a stack-allocated ReadOnlySpan to inspect individual character data without invoking substring allocations.
    /// Checksum verification processes the numerical arrays using arithmetic character conversions by subtracting the constant character zero from each index.
    /// </remarks>
    /// <param name="cpf">The raw CPF string to evaluate, which may include structural punctuation marks.</param>
    /// <returns>True if the document contains a valid length, represents a non-repeating sequence, and satisfies both verification digit formulas; otherwise, false.</returns>
    public static bool ValidCPF(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            return false;

        string cleanCpf = cpf.RemoveNonAlphanumeric();

        if (cleanCpf.Length != 11 || !CPFRegex().IsMatch(cleanCpf))
            return false;

        ReadOnlySpan<char> span = cleanCpf.AsSpan();

        if (span.IsAllSameDigits())
            return false;

        int[] multipliers1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int sum = 0;

        for (int i = 0; i < 9; i++)
            sum += (span[i] - '0') * multipliers1[i];

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        if ((span[9] - '0') != digit1)
            return false;

        int[] multipliers2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
        sum = 0;

        for (int i = 0; i < 10; i++)
            sum += (span[i] - '0') * multipliers2[i];

        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return (span[10] - '0') == digit2;
    }

    /// <summary>
    /// Validates a CNPJ (Brazilian National Registry of Legal Entities number) using a high-performance checksum algorithm.
    /// </summary>
    /// <remarks>
    /// The validation pipeline clears layout formatting and enforces a strict fourteen-digit constraint via the compiled CNPJRegex.
    /// It guards against logical fraudulent entries by running an equality loop across all characters within the input segment.
    /// Arithmetic parsing converts individual characters to integers by applying an unmanaged character offset subtraction method.
    /// Verification uses two sequential loops against distinct weighted arrays to validate the structural terminal digits.
    /// </remarks>
    /// <param name="cnpj">The corporate CNPJ string to validate, which may include dashes, dots, or slashes.</param>
    /// <returns>True if the cleansed corporate identifier meets all regex requirements, contains variant digits, and matches both structural validation checks; otherwise, false.</returns>
    public static bool ValidCNPJ(string cnpj)
    {
        if (string.IsNullOrEmpty(cnpj))
            return false;

        string cleanCnpj = cnpj.RemoveNonAlphanumeric();

        if (cleanCnpj.Length != 14 || !CNPJRegex().IsMatch(cleanCnpj))
            return false;

        ReadOnlySpan<char> span = cleanCnpj.AsSpan();

        if (span.IsAllSameDigits())
            return false;

        int[] multipliers1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int sum = 0;

        for (int i = 0; i < 12; i++)
            sum += (span[i] - '0') * multipliers1[i];

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        if ((span[12] - '0') != digit1)
            return false;

        int[] multipliers2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += (span[i] - '0') * multipliers2[i];

        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return (span[13] - '0') == digit2;
    }

    /// <summary>
    /// Validates a PIS (Brazilian Social Integration Program number) using an optimized single-digit checksum calculation.
    /// </summary>
    /// <remarks>
    /// The string is filtered of structural characters and checked against the compiled PISRegex for an exact eleven-digit match.
    /// Repetitive sequences are rejected through a uniform digit validation helper before computing the checksum.
    /// Modulo eleven arithmetic translates the initial ten digits through specific multiplier weights to verify the final digit.
    /// </remarks>
    /// <param name="pis">The social registration PIS string to evaluate, which can include dashes and dots.</param>
    /// <returns>True if the string forms a valid document format, bypasses uniformity checks, and maps precisely to the calculated terminal verification digit; otherwise, false.</returns>
    public static bool ValidPIS(string pis)
    {
        if (string.IsNullOrEmpty(pis))
            return false;

        string cleanPis = pis.RemoveNonAlphanumeric();

        if (cleanPis.Length != 11 || !PISRegex().IsMatch(cleanPis))
            return false;

        ReadOnlySpan<char> span = cleanPis.AsSpan();

        if (span.IsAllSameDigits())
            return false;

        int[] multipliers = [3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int sum = 0;

        for (int i = 0; i < 10; i++)
            sum += (span[i] - '0') * multipliers[i];

        int remainder = sum % 11;
        int expectedDigit = remainder < 2 ? 0 : 11 - remainder;

        return (span[10] - '0') == expectedDigit;
    }

    #endregion Validates
}