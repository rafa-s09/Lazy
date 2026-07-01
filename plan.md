1. **Document new functions in `StringExtensions.cs`:**
   - I will add standard C# XML comments (summary, remarks, parameters, returns, and example) to `ExtractLetters`.
   - I will add standard C# XML comments to `ExtractNumbers`.
   - I will add standard C# XML comments to the group of `TryParseNumbers` methods (for byte, short, int, long, sbyte, ushort, uint, ulong).
   - I will add standard C# XML comments to `TryParseFloat`.
   - I will add standard C# XML comments to `TryParseDouble`.
   - I will add standard C# XML comments to `TryParseDecimal`.

2. **Add tests for the new functions in `StringExtensionsTests.cs`:**
   - Add test cases for `ExtractLetters` (with and without `replaceWithSpace`).
   - Add test cases for `ExtractNumbers` (with and without `replaceWithSpace`).
   - Add test cases for `TryParseNumbers` for a subset of types (e.g. int, long) verifying both successful parsing of numbers inside strings and failure for invalid strings.
   - Add test cases for `TryParseFloat`, `TryParseDouble`, and `TryParseDecimal` verifying successful parsing (including dot/comma handling) and failure.

3. **Pre-commit step:**
   - Ensure proper testing, verification, review, and reflection are done by calling the pre-commit step.
