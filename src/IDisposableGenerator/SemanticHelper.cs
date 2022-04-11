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

        return string.Join(".", parts);
    }

    public static bool FullNamespaceEquals(this ISymbol symbol, string @namespace)
        => symbol.FullNamespace().Equals(@namespace, StringComparison.Ordinal);

    public static void ToSourceFile(
        this string source,
        string sourceName,
        ref GeneratorExecutionContext context
        )
        => context.AddSource(sourceName, source);

    public static void ToSourceFile(
        this string source,
        string sourceName,
        ref GeneratorPostInitializationContext context
        )
        => context.AddSource(sourceName, source);
}
