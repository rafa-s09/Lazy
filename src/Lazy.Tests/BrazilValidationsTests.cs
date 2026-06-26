using Lazy.Validations.Brazil;

namespace Lazy.Tests;

public class BrazilValidationsTests
{
    [Theory]
    [InlineData("11144477735")] // Valid formatted CPF
    [InlineData("111.444.777-35")] // Valid formatted CPF
    public void ValidCPF_ReturnsTrueForValid(string cpf)
    {
        Assert.True(Validations.Brazil.Validations.ValidCPF(cpf));
    }

    [Theory]

    [InlineData("")]
    [InlineData("11111111111")] // Repeating digits
    [InlineData("123")] // Too short
    [InlineData("111.444.777-36")] // Invalid digit
    public void ValidCPF_ReturnsFalseForInvalid(string cpf)
    {
        Assert.False(cpf == null ? Lazy.Validations.Brazil.Validations.ValidCPF(string.Empty) : Validations.Brazil.Validations.ValidCPF(cpf));
    }

    [Theory]
    [InlineData("11222333000181")] // Valid formatted CNPJ
    [InlineData("11.222.333/0001-81")] // Valid formatted CNPJ
    public void ValidCNPJ_ReturnsTrueForValid(string cnpj)
    {
        Assert.True(Validations.Brazil.Validations.ValidCNPJ(cnpj));
    }

    [Theory]

    [InlineData("")]
    [InlineData("11111111111111")] // Repeating digits
    [InlineData("123")] // Too short
    [InlineData("11.222.333/0001-82")] // Invalid digit
    public void ValidCNPJ_ReturnsFalseForInvalid(string cnpj)
    {
        Assert.False(Lazy.Validations.Brazil.Validations.ValidCNPJ(cnpj ?? string.Empty));
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("123.45678.90-0")]
    public void ValidPIS_ReturnsTrueForValid(string pis)
    {
        // Calculate valid check digit for 1234567890
        // multipliers = [3, 2, 9, 8, 7, 6, 5, 4, 3, 2]
        // 1*3 + 2*2 + 3*9 + 4*8 + 5*7 + 6*6 + 7*5 + 8*4 + 9*3 + 0*2
        // 3 + 4 + 27 + 32 + 35 + 36 + 35 + 32 + 27 + 0 = 231
        // 231 % 11 = 0
        // Expected digit: 11 - 0 = 11? Wait, remainder < 2 ? 0 : 11 - remainder => 0 < 2 -> 0.
        // So 12345678900 should be valid. Let's test that instead.

        Assert.True(Validations.Brazil.Validations.ValidPIS(pis));
    }

    [Theory]

    [InlineData("")]
    [InlineData("11111111111")] // Repeating digits
    [InlineData("123")] // Too short
    [InlineData("12345678901")] // Invalid digit
    public void ValidPIS_ReturnsFalseForInvalid(string pis)
    {
        Assert.False(Lazy.Validations.Brazil.Validations.ValidPIS(pis ?? string.Empty));
    }
}
