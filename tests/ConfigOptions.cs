namespace IDisposableGenerator.Tests;

/// <summary>
/// Allows adding additional global options
/// </summary>
internal class ConfigOptions : AnalyzerConfigOptions
{
    private readonly AnalyzerConfigOptions _workspaceOptions;
    private readonly Dictionary<string, string> _globalOptions;

    [ExcludeFromCodeCoverage]
    public ConfigOptions(AnalyzerConfigOptions workspaceOptions, List<(string, string)> globalOptions)
    {
        this._workspaceOptions = workspaceOptions;
        this._globalOptions = globalOptions.ToDictionary( t => t.Item1, t => t.Item2);
    }

    [ExcludeFromCodeCoverage]
    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        => this._workspaceOptions.TryGetValue(key, out value) || this._globalOptions.TryGetValue(key, out value);
}
