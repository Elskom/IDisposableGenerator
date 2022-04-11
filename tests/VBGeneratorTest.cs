namespace IDisposableGenerator.Tests;

public class VBGeneratorTest : VisualBasicIncrementalGeneratorTest<IDisposableGeneratorVB, XUnitVerifier>, IGeneratorTestBase
{
    public List<(string, string)> GlobalOptions { get; } = new();

    protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
        => VisualBasicGeneratorDriver.Create(
            sourceGenerators,
            project.AnalyzerOptions.AdditionalFiles,
            (VisualBasicParseOptions)project.ParseOptions!,
            new OptionsProvider(project.AnalyzerOptions.AnalyzerConfigOptionsProvider, this.GlobalOptions));
}
