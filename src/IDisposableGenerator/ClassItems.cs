namespace IDisposableGenerator;

internal class ClassItems
{
    public string? Name { get; set; }
    public Accessibility Accessibility { get; set; }
    public bool Stream { get; set; }
    public bool WithoutThrowIfDisposed { get; set; }
    public List<ISymbol> Owns { get; } = [];
    public List<ISymbol> Fields { get; } = [];
    public List<string> SetNull { get; } = [];
    public List<string> Methods { get; } = [];

    public static bool IsReadOnlyField(ISymbol member)
        => (member is IFieldSymbol fieldSymbol && fieldSymbol.IsReadOnly) ||
           (member is IPropertySymbol propertySymbol && propertySymbol.IsReadOnly);

    public bool AddSetNull(ISymbol member)
    {
        if (!IsReadOnlyField(member) || member is IEventSymbol)
        {
            this.SetNull.Add(member.Name);
        }

        return true;
    }

    public bool AddMethod(ISymbol member)
    {
        if (member is IMethodSymbol)
        {
            this.Methods.Add(member.Name);
        }

        return true;
    }

    public bool AddField(TypedConstant arg, ISymbol member)
    {
        if ((bool)arg.Value!)
        {
            this.Owns.Add(member);
        }
        else
        {
            this.Fields.Add(member);
        }

        return true;
    }

    public bool NameEquals(string name)
        => this.Name!.Equals(name, StringComparison.Ordinal);

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        var result = new StringBuilder();
        _ = result.Append($"Class: Name {this.Name}")
            .Append($", Accessibility: {this.Accessibility}")
            .Append($", Stream: {this.Stream}")
            .Append($", Without ThrowIfDisposed: {this.WithoutThrowIfDisposed}")
            .Append($", Owns Count: {this.Owns.Count}")
            .Append($", Fields Count: {this.Fields.Count}")
            .Append($", SetNull Count: {this.SetNull.Count}")
            .Append($", Methods Count: {this.Methods.Count}");
        return result.ToString();
    }
}
