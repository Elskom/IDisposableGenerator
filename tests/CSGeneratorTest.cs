namespace IDisposableGenerator.Tests;

public class CSGeneratorTest : CSharpSourceGeneratorTest<IDisposibleGenerator.IDisposableGenerator, XUnitVerifier>, IGeneratorTestBase
{
    public List<(string, string)> GlobalOptions { get; } = new();

    protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
        => CSharpGeneratorDriver.Create(
            sourceGenerators,
            project.AnalyzerOptions.AdditionalFiles,
            (CSharpParseOptions)project.ParseOptions!,
            new OptionsProvider(project.AnalyzerOptions.AnalyzerConfigOptionsProvider, this.GlobalOptions));
}
