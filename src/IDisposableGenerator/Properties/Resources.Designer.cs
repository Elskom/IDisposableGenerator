namespace IDisposableGenerator.Properties;

internal class Resources
{
    internal static string AttributeCodeCSharp => ResourceManager.GetString(
        "AttributeCodeCSharp");
    
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    private static ResourceManager ResourceManager { get; } = new ResourceManager(
        "IDisposableGenerator.Properties.Resources",
        typeof(Resources).Assembly);
}
