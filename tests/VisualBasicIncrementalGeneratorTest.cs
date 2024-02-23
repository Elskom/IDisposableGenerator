namespace IDisposableGenerator.Tests;

public class VisualBasicIncrementalGeneratorTest<TSourceGenerator, TVerifier> : SourceGeneratorTest<TVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TVerifier : IVerifier, new()
{
    protected override IEnumerable<Type> GetSourceGenerators()
        => [typeof(TSourceGenerator)];

    protected override string DefaultFileExt => "vb";

    public override string Language => LanguageNames.VisualBasic;

    public List<(string, string)> GlobalOptions { get; } = [];

    public Microsoft.CodeAnalysis.VisualBasic.LanguageVersion LanguageVersion { get; set; } = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Default;

    // [ExcludeFromCodeCoverage]
    // protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
    //     => VisualBasicGeneratorDriver.Create(
    //         sourceGenerators,
    //         project.AnalyzerOptions.AdditionalFiles,
    //         (VisualBasicParseOptions)project.ParseOptions!,
    //         project.AnalyzerOptions.AnalyzerConfigOptionsProvider);

    [ExcludeFromCodeCoverage]
    protected override CompilationOptions CreateCompilationOptions()
        => new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

    [ExcludeFromCodeCoverage]
    protected override ParseOptions CreateParseOptions()
        => new VisualBasicParseOptions(this.LanguageVersion, DocumentationMode.Diagnose);

    [ExcludeFromCodeCoverage]
    protected override AnalyzerOptions GetAnalyzerOptions(Project project)
        => new(
            project.AnalyzerOptions.AdditionalFiles,
            new OptionsProvider(project.AnalyzerOptions.AnalyzerConfigOptionsProvider, this.GlobalOptions));
}
