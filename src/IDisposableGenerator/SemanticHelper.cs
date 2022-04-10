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
}
