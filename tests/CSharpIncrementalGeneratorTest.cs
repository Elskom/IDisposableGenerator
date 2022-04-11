namespace IDisposableGenerator.Tests;

public class CSharpIncrementalGeneratorTest<TSourceGenerator, TVerifier> : SourceGeneratorTest<TVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TVerifier : IVerifier, new()
{
    protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
        => new[] { new TSourceGenerator().AsSourceGenerator() };

    protected override string DefaultFileExt => "cs";

    public override string Language => LanguageNames.CSharp;

    [ExcludeFromCodeCoverage]
    protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
        => CSharpGeneratorDriver.Create(
            sourceGenerators,
            project.AnalyzerOptions.AdditionalFiles,
            (CSharpParseOptions)project.ParseOptions!,
            project.AnalyzerOptions.AnalyzerConfigOptionsProvider);

    protected override CompilationOptions CreateCompilationOptions()
        => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);

    protected override ParseOptions CreateParseOptions()
        => new CSharpParseOptions(LanguageVersion.Default, DocumentationMode.Diagnose);
}
