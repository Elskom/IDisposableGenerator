namespace IDisposableGenerator;

internal static class SemanticHelper
{
    public static string FullNamespace(this ISymbol symbol)
    {
        var parts = new Stack<string>();
        var iterator = symbol as INamespaceSymbol ?? symbol.ContainingNamespace;
        while (iterator != null)
        {
            if (!string.IsNullOrEmpty(iterator.Name))
            {
                parts.Push(iterator.Name);
            }

            iterator = iterator.ContainingNamespace;
        }

        return parts.Count == 0 ? string.Empty : string.Join(".", parts);
    }

    public static bool FullNamespaceEquals(this ISymbol symbol, string @namespace)
        => symbol.FullNamespace().Equals(@namespace, StringComparison.Ordinal);

    public static void ToSourceFile(
        this StringBuilder source,
        string sourceName,
        ref SourceProductionContext context)
        => context.AddSource(sourceName, source.ToString());

    public static void ToSourceFile(
        this StringBuilder source,
        string sourceName,
        ref IncrementalGeneratorPostInitializationContext context)
        => context.AddSource(sourceName, source.ToString());
}
