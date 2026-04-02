namespace IDisposableGenerator.Tests;

internal static class SGAssert
{
    internal static async Task RunTest<TestType>(
        string generatedSource,
        string testSource,
        LanguageVersion? languageVersion = LanguageVersion.CSharp12,
        List<string>? testSources = null,
        Dictionary<string, string>? generatedSources = null,
        List<DiagnosticResult>? expectedDiagnostics = null)
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
                tst.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorCS), "GeneratedAttributes.g.cs", Properties.Resources.AttributeCodeCSharp!));
                if (languageVersion < LanguageVersion.CSharp12)
                {
                    tst.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics!);
                }
                if (generatedSources is not null
                    && languageVersion == LanguageVersion.CSharp12)
                {
                    foreach (var source in testSources!)
                    {
                        tst.TestState.Sources.Add(source.ReplaceLineEndings());
                    }

                    foreach (var (key, value) in generatedSources)
                    {
                        tst.TestState.GeneratedSources.Add(
                            (typeof(IDisposableGeneratorCS), key, value.ReplaceLineEndings()));
                    }
                }
                else
                {
                    tst.TestState.GeneratedSources.Add(
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
            case true when test is CSGeneratorTest tst:
            {
                tst.LanguageVersion = languageVersion!.Value;
                tst.TestState.GeneratedSources.Add(
                    (typeof(IDisposableGeneratorCS), "GeneratedAttributes.g.cs", Properties.Resources.AttributeCodeCSharp!));
                tst.TestState.Sources.Clear();
                if (languageVersion < LanguageVersion.CSharp12)
                {
                    tst.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics!);
                }
                break;
            }
        }

        await test.RunAsync();
    }
}
