namespace IDisposableGenerator.Tests;

public class CSharpIncrementalGeneratorTest<TSourceGenerator, TVerifier> : SourceGeneratorTest<TVerifier>
    where TSourceGenerator : IIncrementalGenerator, new()
    where TVerifier : IVerifier, new()
{
    protected override IEnumerable<Type> GetSourceGenerators()
        => [typeof(TSourceGenerator)];

    protected override string DefaultFileExt => "cs";

    public override string Language => LanguageNames.CSharp;

    public List<(string, string)> GlobalOptions { get; } = [];

    public LanguageVersion LanguageVersion { get; set; }

    // [ExcludeFromCodeCoverage]
    // protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
    //     => CSharpGeneratorDriver.Create(
    //         sourceGenerators,
    //         project.AnalyzerOptions.AdditionalFiles,
    //         (CSharpParseOptions)project.ParseOptions!,
    //         project.AnalyzerOptions.AnalyzerConfigOptionsProvider);

    [ExcludeFromCodeCoverage]
    protected override CompilationOptions CreateCompilationOptions()
        => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true);

    [ExcludeFromCodeCoverage]
    protected override ParseOptions CreateParseOptions()
        => new CSharpParseOptions(this.LanguageVersion, DocumentationMode.Diagnose);

    [ExcludeFromCodeCoverage]
    protected override AnalyzerOptions GetAnalyzerOptions(Project project)
        => new(
            project.AnalyzerOptions.AdditionalFiles,
            new OptionsProvider(project.AnalyzerOptions.AnalyzerConfigOptionsProvider, this.GlobalOptions));
}
