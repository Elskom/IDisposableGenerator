namespace IDisposableGenerator.Tests;

using System.Collections.Generic;

public interface IGeneratorTestBase
{
    /// <summary>
    /// Allows you to specify additional global options that will appear in the context.AnalyzerConfigOptions.GlobalOptions object.
    /// </summary>
    public List<(string, string)> GlobalOptions { get; }
}
