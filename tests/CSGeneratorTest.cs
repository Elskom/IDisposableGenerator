namespace IDisposableGenerator.Tests;

public class CSGeneratorTest : CSharpIncrementalGeneratorTest<IDisposableGenerator, XUnitVerifier>, IGeneratorTestBase
{
    // protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
    //     => CSharpGeneratorDriver.Create(
    //         sourceGenerators,
    //         project.AnalyzerOptions.AdditionalFiles,
    //         (CSharpParseOptions)this.CreateParseOptions(),
    //         new OptionsProvider(project.AnalyzerOptions.AnalyzerConfigOptionsProvider, this.GlobalOptions));
}
