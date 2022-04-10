namespace IDisposableGenerator.Properties;

internal class Resources
{
    internal static string AttributeCodeCSharp9 => ResourceManager.GetString(
        "AttributeCodeCSharp9");
    
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    private static ResourceManager ResourceManager { get; } = new ResourceManager(
        "IDisposableGenerator.Properties.Resources",
        typeof(Resources).Assembly);
}
