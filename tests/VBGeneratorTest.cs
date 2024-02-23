namespace IDisposableGenerator.Tests;

public class VBGeneratorTest : VisualBasicIncrementalGeneratorTest<IDisposableGeneratorVB, XUnitVerifier>, IGeneratorTestBase
{
    // protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
    //     => VisualBasicGeneratorDriver.Create(
    //         sourceGenerators,
    //         project.AnalyzerOptions.AdditionalFiles,
    //         (VisualBasicParseOptions)project.ParseOptions!,
    //         new OptionsProvider(project.AnalyzerOptions.AnalyzerConfigOptionsProvider, this.GlobalOptions));
}
