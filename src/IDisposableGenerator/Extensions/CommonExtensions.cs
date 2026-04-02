namespace IDisposableGenerator.Extensions;

internal static class CommonExtensions
{
    extension(ISymbol symbol)
    {
        public bool IsDisposable()
        {
            var type = symbol switch
            {
                IFieldSymbol f => f.Type,
                IPropertySymbol p => p.Type,
                INamedTypeSymbol tp => tp,
                _ => null
            };

            // Look for IDisposable
            return type?.AllInterfaces.Any(
                static i => i.ToDisplayString() == "System.IDisposable") ?? false;
        }

        public bool IsAsyncDisposable()
        {
            var type = symbol switch
            {
                IFieldSymbol f => f.Type,
                IPropertySymbol p => p.Type,
                INamedTypeSymbol tp => tp,
                _ => null
            };

            // Look for IAsyncDisposable
            return type?.AllInterfaces.Any(
                static i => i.ToDisplayString() == "System.IAsyncDisposable") ?? false;
        }

        public (ISymbol Symbol, bool IsAsyncDisposable) GetIsAsyncDisposableTuple()
            => (symbol, symbol.IsAsyncDisposable());

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
                    static (ctx, _) =>
                    {
                        var nts = ctx.TargetSymbol as INamedTypeSymbol;
                        return new ClassSymbolCache
                        {
                            Name = nts!.Name,
                            Namespace = nts!.FullNamespace(),
                            // IsAsyncDisposable = nts!.IsAsyncDisposable(),
                            BaseIsDisposable = nts!.BaseType?.IsDisposable() ?? false,
                            BaseIsAsyncDisposable = nts!.BaseType?.IsAsyncDisposable() ?? false,
                            DeclaredAccessibility = nts!.DeclaredAccessibility,
                            ClassAttributes = nts!.GetAttributes(),
                            MemberAttributes = nts!.GetMembers().ToImmutableDictionary(
                                static member => member,
                                static member => member.GetAttributes(),
                                SymbolEqualityComparer.Default)
                        };
                    })
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
