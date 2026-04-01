namespace IDisposableGenerator.Extensions;

internal static class CommonExtensions
{
    extension(ISymbol symbol)
    {
        public string FullNamespace()
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

        public bool FullNamespaceEquals(string @namespace)
            => symbol.FullNamespace().Equals(@namespace, StringComparison.Ordinal);
    }

    extension(StringBuilder source)
    {
        public void ToSourceFile(
            string sourceName,
            ref SourceProductionContext context)
            => context.AddSource(sourceName, source.ToString());

        public void ToSourceFile(
            string sourceName,
            ref IncrementalGeneratorPostInitializationContext context)
            => context.AddSource(sourceName, source.ToString());
    }

    extension(IncrementalGeneratorInitializationContext context)
    {
        public void RegisterCommonSourceOutput<T>(IncrementalValueProvider<WorkItemCollection> workItemCollection)
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
                    foreach (var (file, code) in items.Right.GeneratedCodeWriter.ToCodeStrings(ctx, items.Right.GetWorkItems().AsReadOnly()))
                    {
                        code.ToSourceFile(file, ref ctx);
                    }
                });
    }
}
