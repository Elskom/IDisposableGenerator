namespace IDisposableGenerator.Tests;

public partial class IDisposableGeneratorTests
{
    private static readonly DiagnosticResult UnsupportedLanguageVersionDiagnostic = DiagnosticResult.CompilerError("IDISPGEN001");

    [Fact]
    public async Task TestGeneratingNoInputEOLCSharp9()
        => await RunTest<CSGeneratorTest>(string.Empty, string.Empty, LanguageVersion.CSharp9, expectedDiagnostics: [UnsupportedLanguageVersionDiagnostic.WithArguments("9.0", "12.0")]);

    [Fact]
    public async Task TestGeneratingNoInputEOLCSharp10()
        => await RunTest<CSGeneratorTest>(string.Empty, string.Empty, LanguageVersion.CSharp10, expectedDiagnostics: [UnsupportedLanguageVersionDiagnostic.WithArguments("10.0", "12.0")]);

    [Fact]
    public async Task TestGeneratingNoInputEOLCSharp11()
        => await RunTest<CSGeneratorTest>(string.Empty, string.Empty, LanguageVersion.CSharp11, expectedDiagnostics: [UnsupportedLanguageVersionDiagnostic.WithArguments("11.0", "12.0")]);
}
