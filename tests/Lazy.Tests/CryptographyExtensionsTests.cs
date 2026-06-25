using Lazy.Extensions;

namespace Lazy.Tests;

public class CryptographyExtensionsTests
{
    [Fact]
    public void Checksums_And_Hashes_ReturnValidResults()
    {
        string input = "hello";

        Assert.NotEqual(0u, input.ToCrc32());
        Assert.NotEqual(0ul, input.ToCrc64());
        Assert.NotEqual(0u, input.ToXxHash32());
        Assert.NotEqual(0ul, input.ToXxHash64());
        Assert.NotEqual(UInt128.Zero, input.ToXxHash128());
        Assert.NotEqual(0ul, input.ToXxHash3());

        Assert.False(string.IsNullOrEmpty(input.ToXxHash3String()));
        Assert.Equal(32, input.ToMd5().Length);
        Assert.Equal(64, input.ToSha256().Length);
    }

    [Fact]
    public void EmptyString_ReturnsZeroOrEmpty()
    {
        string input = "";

        Assert.Equal(0u, input.ToCrc32());
        Assert.Equal(0ul, input.ToCrc64());
        Assert.Equal(0u, input.ToXxHash32());
        Assert.Equal(0ul, input.ToXxHash64());
        Assert.Equal(UInt128.Zero, input.ToXxHash128());
        Assert.Equal(0ul, input.ToXxHash3());

        Assert.Equal(string.Empty, input.ToXxHash3String());
        Assert.Equal(string.Empty, input.ToMd5());
        Assert.Equal(string.Empty, input.ToSha256());
    }

    [Fact]
    public void GenerateRandomToken_ReturnsCorrectLength()
    {
        string token = CryptographyExtensions.GenerateRandomToken(16);
        Assert.Equal(16, token.Length);
    }

    [Fact]
    public void GenerateRandomToken_InvalidArguments_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CryptographyExtensions.GenerateRandomToken(0));
        Assert.Throws<ArgumentException>(() => CryptographyExtensions.GenerateRandomToken(16, ""));
        Assert.Throws<ArgumentNullException>(() => CryptographyExtensions.GenerateRandomToken(16, null!));
    }

    [Fact]
    public void GenerateShortId_ReturnsExpectedLength()
    {
        string id = CryptographyExtensions.GenerateShortId();
        Assert.Equal(22, id.Length);
    }
}
