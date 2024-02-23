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
        where TestType : SourceGeneratorTest<XUnitVerifier>, IGeneratorTestBase, new()
    {
        var test = new TestType
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            TestState =
            {
                Sources =
                {
                    testSource
                },
            },
        };

        switch (string.IsNullOrEmpty(testSource))
        {
            case false when test is CSGeneratorTest tst:
            {
                tst.LanguageVersion = languageVersion!.Value;
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGenerator), "GeneratedAttributes.g.cs", Properties.Resources.AttributeCodeCSharp!));
                if (generatedSources is not null
                    && languageVersion == LanguageVersion.CSharp10)
                {
                    foreach (var source in testSources!)
                    {
                        test.TestState.Sources.Add(source);
                    }

                    foreach (var (key, value) in generatedSources)
                    {
                        test.TestState.GeneratedSources.Add(
                            (typeof(IDisposableGenerator), key, value));
                    }
                }
                else
                {
                    test.TestState.GeneratedSources.Add(
                        (typeof(IDisposableGenerator), "Disposables.g.cs", generatedSource));
                }

                break;
            }
            case false when test is VBGeneratorTest:
            {
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorVB), "GeneratedAttributes.g.vb", Properties.Resources.AttributeCodeVisualBasic!));
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorVB), "Disposables.g.vb", generatedSource));
                break;
            }
            default:
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGenerator), "GeneratedAttributes.g.cs", Properties.Resources.AttributeCodeCSharp!));
                test.TestState.Sources.Clear();
                break;
        }

        await test.RunAsync();
    }
}
