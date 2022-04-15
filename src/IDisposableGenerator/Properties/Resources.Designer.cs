namespace IDisposableGenerator.Properties;

internal class Resources
{
    internal static string AttributeCodeCSharp => ResourceManager.GetString(
        nameof(AttributeCodeCSharp));

    internal static string AttributeCodeVisualBasic => ResourceManager.GetString(
        nameof(AttributeCodeVisualBasic));

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    private static ResourceManager ResourceManager { get; } = new ResourceManager(
        "IDisposableGenerator.Properties.Resources",
        typeof(Resources).Assembly);
}
