namespace IDisposableGenerator;

internal record struct ClassSymbolCache
{
    internal string Name { get; set; }
    internal string Namespace { get; set; }
    internal Accessibility DeclaredAccessibility { get; set; }
    internal ImmutableArray<AttributeData> ClassAttributes { get; set; }
    internal ImmutableDictionary<ISymbol, ImmutableArray<AttributeData>> MemberAttributes { get; set; }
}
