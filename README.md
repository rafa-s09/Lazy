# Lazy

> Because developers are too lazy to rewrite the same C# extension methods over and over again.

**Lazy** is a high-performance .NET 10.0 C# utility library designed to prevent reinventing the wheel. It provides an essential set of extension methods, common validations, and high-performance, allocation-free data structures that C# developers frequently use across different projects.

## Features

### 🇧🇷 Brazilian Document Validations
Built-in, zero-allocation validations for common Brazilian documents:
- **CPF** (Cadastro de Pessoas Físicas)
- **CNPJ** (Cadastro Nacional da Pessoa Jurídica)
- **PIS** (Programa de Integração Social)

```csharp
using Lazy.Validations.Brazil;

bool isValidCpf = "123.456.789-00".IsValidCpf();
bool isValidCnpj = "12.345.678/0001-90".IsValidCnpj();
```

### 🚀 High-Performance Data Structures (`BufferList<T>`)
A high-performance, allocation-free circular buffer list that operates entirely on `Span<T>`. It is designed for scenarios where minimizing Garbage Collection (GC) pressure is critical.

```csharp
using Lazy.Buffer;

// Allocate memory on the stack (no heap allocations!)
Span<int> buffer = stackalloc int[5];
Span<bool> validation = stackalloc bool[5];

// Initialize the buffer list
var list = new BufferList<int>(buffer, validation);

list.Append(10);
list.Append(20);
list.Append(30);

// Use familiar methods
int first = list.PeekFirst(); // 10
bool found = list.TryFind(x => x == 20, out int result); // true
list.RemoveFirst(); // removes 10
```

### 🛠️ Extension Methods
A comprehensive suite of extension methods targeting common types:
- **Strings:** Conversions, manipulation, base64 encoding/decoding.
- **Numerics:** Safe conversions, formatting.
- **Date & Time:** Common date manipulations, truncations.
- **Cryptography:** Easy hashing and encryption helpers.

## Requirements

- .NET 10.0 or later

## Building and Testing

To build the project and run the extensive xUnit test suite:

```bash
cd src
dotnet build
dotnet test
```

## License
MIT