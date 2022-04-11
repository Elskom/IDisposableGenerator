namespace IDisposableGenerator.Tests;

public class VisualBasicIncrementalGeneratorTest<TSourceGenerator, TVerifier> : SourceGeneratorTest<TVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TVerifier : IVerifier, new()
{
    protected override IEnumerable<ISourceGenerator> GetSourceGenerators()
        => new[] { new TSourceGenerator().AsSourceGenerator() };

    protected override string DefaultFileExt => "vb";

    public override string Language => LanguageNames.VisualBasic;

    [ExcludeFromCodeCoverage]
    protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
        => VisualBasicGeneratorDriver.Create(
            sourceGenerators,
            project.AnalyzerOptions.AdditionalFiles,
            (VisualBasicParseOptions)project.ParseOptions!,
            project.AnalyzerOptions.AnalyzerConfigOptionsProvider);

    protected override CompilationOptions CreateCompilationOptions()
        => new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

    protected override ParseOptions CreateParseOptions()
        => new VisualBasicParseOptions(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Default, DocumentationMode.Diagnose);
}
