namespace IDisposableGenerator.Tests;

public partial class IDisposableGeneratorTests
{
    [Fact]
    public async Task TestGeneratingNoInput()
        => _ = await Assert.ThrowsAsync<EqualWithMessageException>([ExcludeFromCodeCoverage] async () =>
        {
            await RunTest<CSGeneratorTest>(string.Empty, string.Empty).ConfigureAwait(false);
        }).ConfigureAwait(false);

    private static async Task RunTest<TestType>(
        string generatedSource,
        string testSource,
        LanguageVersion? languageVersion = LanguageVersion.CSharp9)
        where TestType : SourceGeneratorTest<XUnitVerifier>, IGeneratorTestBase, new()
    {
        var test = new TestType
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net60,
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
                var generatedAttributeSource = Properties.Resources.AttributeCodeCSharp!;
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGenerator), "GeneratedAttributes.g.cs", generatedAttributeSource));
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGenerator), "Disposables.g.cs", generatedSource));
                break;
            }
            case false when test is VBGeneratorTest:
            {
                var generatedAttributeSource = Properties.Resources.AttributeCodeVisualBasic!;
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorVB), "GeneratedAttributes.g.vb", generatedAttributeSource));
                test.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorVB), "Disposables.g.vb", generatedSource));
                break;
            }
            default:
                test.TestState.Sources.Clear();
                break;
        }

        await test.RunAsync();
    }
}
