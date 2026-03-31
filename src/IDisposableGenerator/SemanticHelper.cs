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

    public static void RegisterSourceOutput<T>(IncrementalGeneratorInitializationContext context, IncrementalValueProvider<WorkItemCollection> workItemCollection)
        => context.RegisterSourceOutput(
            context.SyntaxProvider.ForAttributeWithMetadataName(
                "IDisposableGenerator.GenerateDisposeAttribute",
                static (n, _) => n is T,
                static (ctx, _) => ctx.TargetSymbol as INamedTypeSymbol)
            .Collect().Combine(workItemCollection),
            static (ctx, items) =>
            {
                items.Right.Process(items.Left, ctx.CancellationToken);

                // begin creating the source we'll inject into the users compilation
                foreach (var (file, code) in items.Right.GeneratedCodeWriter.ToCodeStrings(items.Right.GetWorkItems().AsReadOnly()))
                {
                    code.ToSourceFile(file, ref ctx);
                }
            });
}
