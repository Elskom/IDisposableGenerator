namespace IDisposableGenerator.Tests;

public partial class IDisposableGeneratorTests
{
    [Fact]
    public async Task TestGeneratingNoInput()
        => await RunTest<CSGeneratorTest>(string.Empty, string.Empty);

    private static async Task RunTest<TestType>(
        string generatedSource,
        string testSource,
        LanguageVersion? languageVersion = LanguageVersion.CSharp9,
        List<string>? testSources = null,
        Dictionary<string, string>? generatedSources = null)
        where TestType : SourceGeneratorTest<DefaultVerifier>, IGeneratorTestBase, new()
    {
        var test = new TestType
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net100,
            TestState =
            {
                Sources =
                {
                    testSource.ReplaceLineEndings()
                },
            },
        };

        switch (string.IsNullOrEmpty(testSource))
        {
            case false when test is CSGeneratorTest tst:
            {
                tst.LanguageVersion = languageVersion!.Value;
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorCS), "GeneratedAttributes.g.cs", Properties.Resources.AttributeCodeCSharp!));
                if (generatedSources is not null
                    && languageVersion == LanguageVersion.CSharp10)
                {
                    foreach (var source in testSources!)
                    {
                        test.TestState.Sources.Add(source.ReplaceLineEndings());
                    }

                    foreach (var (key, value) in generatedSources)
                    {
                        test.TestState.GeneratedSources.Add(
                            (typeof(IDisposableGeneratorCS), key, value.ReplaceLineEndings()));
                    }
                }
                else
                {
                    test.TestState.GeneratedSources.Add(
                        (typeof(IDisposableGeneratorCS), "Disposables.g.cs", generatedSource.ReplaceLineEndings()));
                }

                break;
            }
            case false when test is VBGeneratorTest:
            {
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorVB), "GeneratedAttributes.g.vb", Properties.Resources.AttributeCodeVisualBasic!));
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorVB), "Disposables.g.vb", generatedSource.ReplaceLineEndings()));
                break;
            }
            default:
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorCS), "GeneratedAttributes.g.cs", Properties.Resources.AttributeCodeCSharp!));
                test.TestState.Sources.Clear();
                break;
        }

        await test.RunAsync();
    }
}
